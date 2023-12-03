using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sistema.Ferreteria.Core.Cliente.Dominio;
using System.Data;

namespace Sistema.Ferreteria.Core.Cliente.Infraestructura
{
    public class PgsqlClienteRepository : IClienteRepository
    {

        private readonly ILogger<PgsqlClienteRepository> _logger;
        private readonly IConfiguration _config;

        public PgsqlClienteRepository(ILogger<PgsqlClienteRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<int> Create(ClienteModel cliente)
        {
            int id = 0;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();
                id = await dbConnection.ExecuteScalarAsync<int>(
                    "insert into cliente (cli_nombre, cli_cedula, cli_direccion, cli_telefono, cli_correo) values " +
                    "(@Nombre, @Cedula, @Direccion, @Telefono, @Correo) RETURNING cli_id",
                    new
                    {
                        cliente.Nombre,
                        cliente.Cedula,
                        cliente.Direccion,
                        cliente.Telefono,
                        cliente.Correo
                    });
            }
            return id;
        }

        public async Task<int> Delete(string cedula)
        {
            int rowsAffected;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();
                rowsAffected = await dbConnection.ExecuteAsync(
                    "update cliente set cli_estado = 0 where cli_cedula = @Cedula",
                    new { Cedula = cedula });
            }
            return rowsAffected;
        }

        public async Task<ClienteModel?> Get(string cedula)
        {
            ClienteModel? cliente = null;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();
                cliente = await dbConnection.QueryFirstOrDefaultAsync<ClienteModel>(
                    "select cli_id as Id, cli_nombre as Nombre, cli_cedula as Cedula, cli_direccion as Direccion, " +
                    "cli_telefono as Telefono, cli_correo as Correo, cli_estado as Estado " +
                    "from cliente where cli_cedula = @Cedula and cli_estado = 1",
                    new { Cedula = cedula });
            }
            return cliente;
        }

        public async Task<int> Update(ClienteModel cliente)
        {
            int rowsAffected;
            using (IDbConnection dbConnection = new NpgsqlConnection(_config.GetConnectionString("db_ferreteria")))
            {
                dbConnection.Open();
                rowsAffected = await dbConnection.ExecuteAsync(
                    "update cliente set cli_nombre = @Nombre, cli_direccion = @Direccion, " +
                    "cli_telefono = @Telefono, cli_correo = @Correo, cli_estado = @Estado where cli_cedula = @Cedula",
                    new
                    {
                        cliente.Nombre,
                        cliente.Direccion,
                        cliente.Telefono,
                        cliente.Correo,
                        cliente.Estado,
                        cliente.Cedula
                    });
            }
            return rowsAffected;
        }
    }
}
