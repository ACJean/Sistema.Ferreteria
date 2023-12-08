using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sistema.Ferreteria.Core.Seguridad.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Seguridad.Infraestructura
{
    public class PgsqlUsuarioRepository : IUsuarioRepository
    {

        private readonly ILogger<PgsqlUsuarioRepository> _logger;
        private readonly IConfiguration _config;

        public PgsqlUsuarioRepository(ILogger<PgsqlUsuarioRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<UsuarioModel?> Get(string usuario, string clave)
        {
            UsuarioModel? usuarioModel = null;
            using (IDbConnection dbConnection = new NpgsqlConnection(Environment.GetEnvironmentVariable("db_ferreteria")))
            {
                dbConnection.Open();
                usuarioModel = await dbConnection.QueryFirstOrDefaultAsync<UsuarioModel>(
                    "select usu_id as Id, usu_nombre as Nombre, usu_correo as Correo, usu_rol as Rol, cli_cedula as Cedula " +
                    "from usuario left join cliente on usu_cliente_id = cli_id where (usu_nombre = @Usuario or usu_correo = @Usuario) and usu_clave = @Clave",
                    new { Usuario = usuario, Clave = clave });
            }
            return usuarioModel;
        }

        public async Task<int> Register(UsuarioModel usuario)
        {
            int id = 0;
            using (IDbConnection dbConnection = new NpgsqlConnection(Environment.GetEnvironmentVariable("db_ferreteria")))
            {
                dbConnection.Open();
                id = await dbConnection.ExecuteScalarAsync<int>(
                    "insert into usuario (usu_nombre, usu_correo, usu_clave, usu_rol) values " +
                    "(@Nombre, @Correo, @Clave, @Rol) RETURNING usu_id",
                    new {
                        usuario.Nombre,
                        usuario.Correo,
                        usuario.Clave,
                        usuario.Rol
                    });
            }
            return id;
        }
    }
}
