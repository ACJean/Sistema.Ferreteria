using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sistema.Ferreteria.Core.Venta.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Venta.Infraestructura
{
    public class PgsqlCuentaRepository : ICuentaRepository
    {

        private readonly ILogger<PgsqlCuentaRepository> _logger;
        private readonly IConfiguration _config;

        public PgsqlCuentaRepository(ILogger<PgsqlCuentaRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<List<CuentaModel>> Get(FiltroCuentaModel filtro)
        {
            List<CuentaModel>? cuentas;
            using (IDbConnection dbConnection = new NpgsqlConnection(Environment.GetEnvironmentVariable("db_ferreteria")))
            {
                dbConnection.Open();

                cuentas = (await dbConnection.QueryAsync<CuentaModel>("select cue_id as Id, cue_cliente_id as ClienteId, COALESCE(cli_cedula, '9999999999') as ClienteCedula, (cue_fecha_emision + cue_hora_emision) as FechaEmision from cuenta " +
                    "left join cliente on cuenta.cue_cliente_id = cliente.cli_id " +
                    "where @FiltrarFechas = false or (@FiltrarFechas = true and cue_fecha_emision >= @FechaInicio and cue_fecha_emision <= @FechaFinal)",
                    filtro)).ToList();

                List<DetalleCuentaModel> detalles = (await dbConnection.QueryAsync<DetalleCuentaModel>(
                    "select dcu.dcu_id as Id, dcu.dcu_cuenta_id as CuentaId, dcu.dcu_tipo as Tipo, " +
                    "dcu.dcu_articulo_id as ArticuloId, dcu.dcu_cantidad as Cantidad, dcu.dcu_total as Total " +
                    "from detalle_cuenta as dcu inner join cuenta as cue on cue.cue_id = dcu.dcu_cuenta_id " +
                    "where @FiltrarFechas = false or (@FiltrarFechas = true and cue.cue_fecha_emision >= @FechaInicio and cue.cue_fecha_emision <= @FechaFinal)",
                    filtro)).ToList();

                cuentas.ForEach(c =>
                {
                    c.Detalles = detalles.Where(d => c.Id == d.CuentaId).ToList();
                });
            }
            return cuentas;
        }

        public async Task<List<CuentaModel>> Get(int usuarioId)
        {
            List<CuentaModel>? cuentas;
            using (IDbConnection dbConnection = new NpgsqlConnection(Environment.GetEnvironmentVariable("db_ferreteria")))
            {
                dbConnection.Open();

                cuentas = (await dbConnection.QueryAsync<CuentaModel>("select cue_id as Id, cue_cliente_id as ClienteId, COALESCE(cli_cedula, '9999999999') as ClienteCedula, (cue_fecha_emision + cue_hora_emision) as FechaEmision " +
                    "from cuenta left join cliente on cuenta.cue_cliente_id = cliente.cli_id inner join usuario on cliente.cli_id = usuario.usu_cliente_id " +
                    "where usuario.usu_id = @UsuarioId",
                    new { UsuarioId = usuarioId })).ToList();

                List<DetalleCuentaModel> detalles = (await dbConnection.QueryAsync<DetalleCuentaModel>(
                    "select dcu.dcu_id as Id, dcu.dcu_cuenta_id as CuentaId, dcu.dcu_tipo as Tipo, " +
                    "dcu.dcu_articulo_id as ArticuloId, dcu.dcu_cantidad as Cantidad, dcu.dcu_total as Total " +
                    "from detalle_cuenta as dcu inner join cuenta as cue on cue.cue_id = dcu.dcu_cuenta_id " +
                    "left join cliente as cli on cue.cue_cliente_id = cli.cli_id inner join usuario as usu on cli.cli_id = usu.usu_cliente_id " +
                    "where usu.usu_id = @UsuarioId",
                    new { UsuarioId = usuarioId })).ToList();

                cuentas.ForEach(c =>
                {
                    c.Detalles = detalles.Where(d => c.Id == d.CuentaId).ToList();
                });
            }
            return cuentas;
        }

        public async Task<int> Save(CuentaModel cuenta)
        {
            int id = 0;
            using (IDbConnection dbConnection = new NpgsqlConnection(Environment.GetEnvironmentVariable("db_ferreteria")))
            {
                dbConnection.Open();
                using IDbTransaction dbTransaction = dbConnection.BeginTransaction();

                try
                {
                    id = await dbConnection.ExecuteScalarAsync<int>(
                    "insert into cuenta (cue_cliente_id, cue_fecha_emision, cue_hora_emision, cue_subtotal, cue_impuestos, cue_total) values " +
                    "(@ClienteId, @FechaEmision, @HoraEmision, @SubTotal, @Impuestos, @Total) RETURNING cue_id",
                    new
                    {
                        cuenta.ClienteId,
                        FechaEmision = cuenta.FechaEmision.GetValueOrDefault(),
                        HoraEmision = cuenta.FechaEmision.GetValueOrDefault().TimeOfDay,
                        cuenta.Subtotal,
                        cuenta.Impuestos,
                        cuenta.Total,
                    }, dbTransaction);

                    cuenta.Detalles.ForEach(d => d.CuentaId = id);
                    await dbConnection.ExecuteAsync(
                        "insert into detalle_cuenta (dcu_cuenta_id, dcu_tipo, dcu_articulo_id, dcu_cantidad, dcu_total) values " +
                        "(@CuentaId, @Tipo, @ArticuloId, @Cantidad, @Total)",
                        cuenta.Detalles, dbTransaction);

                    await dbConnection.ExecuteAsync(
                        "update articulo set art_stock = art_stock - @Cantidad where art_id = @ArticuloId",
                        cuenta.Detalles, dbTransaction);

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
    }
}
