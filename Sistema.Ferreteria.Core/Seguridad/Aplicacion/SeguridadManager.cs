using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Sistema.Ferreteria.Core.Compartido.Dominio;
using Sistema.Ferreteria.Core.Seguridad.Dominio;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Seguridad.Aplicacion
{
    public class SeguridadManager
    {

        private readonly ILogger<SeguridadManager> _logger;
        private readonly IConfiguration _config;
        private readonly IUsuarioRepository _usuarioRepository;

        public SeguridadManager(ILogger<SeguridadManager> logger, IConfiguration config, IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _config = config;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<TokenModel> Autenticar(AutenticacionModel model)
        {
            UsuarioModel? usuario = await _usuarioRepository.Get(model.Usuario, model.Clave);

            if (usuario == null) return new TokenModel("") 
            { 
                Codigo = 401, 
                Mensaje = "El usuario o la contraseña son incorrectos." 
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                new Claim[]
                {
                    new Claim("name", usuario.Correo),
                    new Claim("id", usuario.Id.ToString()),
                    new Claim("role", usuario.Rol)
                },
                expires: DateTime.Now.AddMinutes(_config.GetSection("Jwt:TimeOut").Get<int>()),
                signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return new TokenModel(token)
            {
                Codigo = 200,
                Mensaje = "Autenticado con éxito."
            };
        }

        public async Task<RespuestaModel> Registrar(UsuarioModel usuario)
        {
            RespuestaModel respuesta = new RespuestaModel();
            try
            {
                usuario.Id = await _usuarioRepository.Register(usuario);
                if (usuario.Id == 0) 
                {
                    respuesta.Codigo = 500;
                    respuesta.Mensaje = "Ocurrió un problema al registrarse, intentelo de nuevo.";
                    return respuesta;
                }

                respuesta.Codigo = 201;
                respuesta.Mensaje = "Registro con éxito.";
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
