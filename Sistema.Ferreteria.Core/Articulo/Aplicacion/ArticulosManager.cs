using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sistema.Ferreteria.Core.Articulo.Dominio;
using Sistema.Ferreteria.Core.Compartido.Dominio;

namespace Sistema.Ferreteria.Core.Articulo.Aplicacion
{
    public class ArticulosManager
    {

        private readonly ILogger<ArticulosManager> _logger;
        private readonly IConfiguration _config;
        private readonly IArticuloRepository _articuloRepository;

        public ArticulosManager(ILogger<ArticulosManager> logger, IConfiguration config, IArticuloRepository articuloRepository)
        {
            _logger = logger;
            _config = config;
            _articuloRepository = articuloRepository;
        }

        public async Task<RespuestaModel> Actualizar(ArticuloModel articulo, int usuarioId)
        {
            RespuestaModel respuesta = new();
            try
            {
                foreach (ArticuloImagenModel articuloImagen in articulo.Imagenes)
                {
                    articuloImagen.Imagen = Convert.FromBase64String(articuloImagen.ImagenBase64);
                }

                ArticuloTraceModel trace = new ArticuloTraceModel
                {
                    Descripcion = "Actualización",
                    Fecha = DateTime.Now,
                    UsuarioId = usuarioId
                };

                int filasAfectadas = await _articuloRepository.Update(articulo, trace);
                
                if (filasAfectadas <= 0)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se actualizó el articulo, intentelo de nuevo.";
                    return respuesta;
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Articulo actualizado.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Baja(int codigo, int usuarioId)
        {
            RespuestaModel respuesta = new();
            try
            {
                ArticuloModel? articulo = await _articuloRepository.Get(codigo);

                if (articulo == null)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se encontró el articulo";
                    respuesta.Datos = null;
                    return respuesta;
                }

                articulo.Estado = 2;

                ArticuloTraceModel trace = new ArticuloTraceModel
                {
                    Descripcion = "Dado de Baja",
                    Fecha = DateTime.Now,
                    UsuarioId = usuarioId
                };

                int filasAfectadas = await _articuloRepository.Update(articulo, trace);

                if (filasAfectadas <= 0)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se pudo dar de baja al articulo, intentelo de nuevo.";
                    return respuesta;
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Articulo dado de baja.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Crear(ArticuloModel articulo, int usuarioId)
        {
            RespuestaModel respuesta = new();
            try
            {
                articulo.Estado = 1;
                foreach (ArticuloImagenModel articuloImagen in articulo.Imagenes)
                {
                    articuloImagen.Imagen = Convert.FromBase64String(articuloImagen.ImagenBase64);
                }

                ArticuloTraceModel trace = new ArticuloTraceModel
                {
                    Descripcion = "Creación",
                    Fecha = DateTime.Now,
                    UsuarioId = usuarioId
                };

                int articuloId = await _articuloRepository.Create(articulo, trace);
                articulo.Id = articuloId;

                respuesta.Codigo = 201;
                respuesta.Mensaje = "Articulo creado.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Eliminar(int id, int usuarioId)
        {
            RespuestaModel respuesta = new();
            try
            {
                ArticuloTraceModel trace = new ArticuloTraceModel
                {
                    Descripcion = "Eliminado",
                    Fecha = DateTime.Now,
                    UsuarioId = usuarioId
                };

                int filasAfectadas = await _articuloRepository.Delete(id, trace);

                if (filasAfectadas <= 0)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se eliminó el articulo, intentelo de nuevo.";
                    return respuesta;
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Articulo eliminado.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Obtener(int codigo)
        {
            RespuestaModel respuesta = new();
            try
            {

                ArticuloModel? articulo = await _articuloRepository.Get(codigo);

                if (articulo == null)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se encontró el articulo";
                    respuesta.Datos = null;
                    return respuesta;
                }

                foreach (ArticuloImagenModel articuloImagen in articulo.Imagenes)
                {
                    articuloImagen.ImagenBase64 = Convert.ToBase64String(articuloImagen.Imagen);
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Articulo encontrado.";
                respuesta.Datos = articulo;
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Obtener()
        {
            RespuestaModel respuesta = new();
            try
            {

                List<ArticuloModel> articulos = await _articuloRepository.Get();

                articulos.ForEach(articulo =>
                {
                    articulo.Imagenes.ForEach(articuloImagen => articuloImagen.ImagenBase64 = Convert.ToBase64String(articuloImagen.Imagen));
                });

                respuesta.Codigo = 200;
                respuesta.Mensaje = $"{articulos.Count} articulos encontrados.";
                respuesta.Datos = articulos;
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

    }
}