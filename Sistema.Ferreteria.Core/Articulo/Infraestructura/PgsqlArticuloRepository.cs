using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sistema.Ferreteria.Core.Articulo.Dominio;
using Sistema.Ferreteria.Core.Seguridad.Dominio;
using System.Data;
using System.Diagnostics;

namespace Sistema.Ferreteria.Core.Articulo.Infraestructura
{
    public class PgsqlArticuloRepository : IArticuloRepository
    {

        private readonly ILogger<PgsqlArticuloRepository> _logger;
        private readonly IConfiguration _config;

        public PgsqlArticuloRepository(ILogger<PgsqlArticuloRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<int> Create(ArticuloModel articulo, ArticuloTraceModel trace)
        {
            int id = 0;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();
                using IDbTransaction dbTransaction = dbConnection.BeginTransaction();

                try
                {
                    id = await dbConnection.ExecuteScalarAsync<int>(
                    "insert into articulo (art_codigo, art_nombre, art_material, art_durabilidad, art_peso, art_tamanio, art_precio, art_stock) values " +
                    "(@Codigo, @Nombre, @Material, @Durabilidad, @Peso, @Tamanio, @Precio, @Stock) RETURNING art_id",
                    new
                    {
                        articulo.Codigo,
                        articulo.Nombre,
                        articulo.Material,
                        articulo.Durabilidad,
                        articulo.Peso,
                        articulo.Tamanio,
                        articulo.Precio,
                        articulo.Stock
                    }, dbTransaction);

                    object[] imagenes = articulo.Imagenes.Select(img => new { ArticuloId = id, img.Imagen }).ToArray();
                    await dbConnection.ExecuteAsync(
                        "insert into articulo_imagen (aim_articulo_id, aim_img) values (@ArticuloId, @Imagen)", 
                        imagenes, dbTransaction);

                    await dbConnection.ExecuteAsync(
                        "insert into articulo_trace (atr_articulo_id, atr_descripcion, art_fecha, art_usuario_id) values " +
                        "(@ArticuloId, @Descripcion, @Fecha, @UsuarioId)",
                        new { ArticuloId = id, trace.Descripcion, trace.Fecha, trace.UsuarioId }, dbTransaction);

                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    throw;
                }

            }
            return id;
        }

        public async Task<int> Delete(int id, ArticuloTraceModel trace)
        {
            int rowsAffected;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();

                using IDbTransaction dbTransaction = dbConnection.BeginTransaction();

                try
                {
                    rowsAffected = await dbConnection.ExecuteAsync(
                    "update articulo set art_estado = 0 where art_id = @Id and art_stock = 0",
                    new { Id = id });

                    await dbConnection.ExecuteAsync(
                        "insert into articulo_trace (atr_articulo_id, atr_descripcion, art_fecha, art_usuario_id) values " +
                        "(@ArticuloId, @Descripcion, @Fecha, @UsuarioId)",
                        new { ArticuloId = id, trace.Descripcion, trace.Fecha, trace.UsuarioId });

                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    rowsAffected = 0;
                    throw;
                }

            }
            return rowsAffected;
        }

        public async Task<ArticuloModel?> Get(int codigo)
        {
            ArticuloModel? articulo = null;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();
                articulo = await dbConnection.QueryFirstOrDefaultAsync<ArticuloModel>(
                    "select art_id as Id, art_codigo as Codigo, art_nombre as Nombre, art_material as Material, " +
                    "art_durabilidad as Durabilidad, art_peso as Peso, art_tamanio as Tamanio, art_precio as Precio, " +
                    "art_stock as Stock, art_estado as Estado " +
                    "from articulo where art_codigo = @Codigo and art_estado = 1",
                    new { Codigo = codigo });

                if (articulo != null) articulo.Imagenes = (await dbConnection.QueryAsync<ArticuloImagenModel>(
                    "select aim_id as Id, aim_articulo_id as ArticuloId, aim_img as Imagen from articulo_imagen where aim_articulo_id = @ArticuloId", 
                    new { ArticuloId = articulo.Id })).ToList();
            }
            return articulo;
        }

        public async Task<List<ArticuloModel>> Get()
        {
            List<ArticuloModel> articulos;
            List<ArticuloImagenModel> imagenesArticulos;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();
                articulos = (await dbConnection.QueryAsync<ArticuloModel>(
                    "select art_id as Id, art_codigo as Codigo, art_nombre as Nombre, art_material as Material, " +
                    "art_durabilidad as Durabilidad, art_peso as Peso, art_tamanio as Tamanio, art_precio as Precio, " +
                    "art_stock as Stock, art_estado as Estado from articulo where art_estado = 1")).ToList();

                imagenesArticulos = (await dbConnection.QueryAsync<ArticuloImagenModel>(
                    "select aim_id as Id, aim_articulo_id as ArticuloId, aim_img as Imagen from articulo_imagen")).ToList();

                articulos.ForEach(articulo =>
                {
                    articulo.Imagenes = imagenesArticulos.Where(articuloImagen => articuloImagen.ArticuloId == articulo.Id).ToList();
                });
            }
            return articulos;
        }

        public async Task<int> Update(ArticuloModel articulo, ArticuloTraceModel trace)
        {
            int rowsAffected;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();

                using IDbTransaction dbTransaction = dbConnection.BeginTransaction();

                try
                {
                    rowsAffected = await dbConnection.ExecuteAsync(
                    "update articulo set art_nombre = @Nombre, art_material = @Material, " +
                    "art_durabilidad = @Durabilidad, art_peso = @Peso, art_tamanio = @Tamanio, art_precio = @Precio, " +
                    "art_stock = @Stock, art_estado = @Estado where art_id = @Id",
                    new
                    {
                        articulo.Nombre,
                        articulo.Material,
                        articulo.Durabilidad,
                        articulo.Peso,
                        articulo.Tamanio,
                        articulo.Precio,
                        articulo.Stock,
                        articulo.Estado,
                        articulo.Id
                    }, dbTransaction);

                    object[] imagenesInsertar = articulo.Imagenes
                        .Where(img => img.Id == null)
                        .Select(img => new { ArticuloId = articulo.Id, img.Imagen })
                        .ToArray();
                    if (imagenesInsertar.Length > 0) await dbConnection.ExecuteAsync(
                        "insert into articulo_imagen (aim_articulo_id, aim_img) values (@ArticuloId, @Imagen)",
                        imagenesInsertar, dbTransaction);

                    object[] imagenesActualizar = articulo.Imagenes
                        .Where(img => img.Id != null)
                        .Select(img => new { img.Id, img.Imagen })
                        .ToArray();
                    if (imagenesActualizar.Length > 0) await dbConnection.ExecuteAsync(
                        "update articulo_imagen set aim_img = @Imagen where aim_id = @Id",
                        imagenesActualizar);

                    await dbConnection.ExecuteAsync(
                        "insert into articulo_trace (atr_articulo_id, atr_descripcion, art_fecha, art_usuario_id) values " +
                        "(@ArticuloId, @Descripcion, @Fecha, @UsuarioId)",
                    new { ArticuloId = articulo.Id, trace.Descripcion, trace.Fecha, trace.UsuarioId });

                    dbTransaction.Commit();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    rowsAffected = 0;
                    throw;
                }
            }
            return rowsAffected;
        }
    }
}
