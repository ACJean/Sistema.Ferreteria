using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Ferreteria.Core.Compartido.Dominio;
using Sistema.Ferreteria.Core.Seguridad.Aplicacion;
using Sistema.Ferreteria.Core.Seguridad.Dominio;
using System.Security.Claims;

namespace Sistema.Ferreteria.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("seguridad")]
    public class SeguridadController : ControllerBase
    {

        private readonly SeguridadManager _seguridadManager;

        public SeguridadController(SeguridadManager seguridadManager)
        {
            _seguridadManager = seguridadManager;
        }

        [HttpPost]
        [Route("autenticar")]
        public async Task<IActionResult> Autenticar([FromBody] AutenticacionModel model)
        {
            TokenModel token = await _seguridadManager.Autenticar(model);
            return StatusCode(token.Codigo, token);
        }

        [HttpPost]
        [Route("registrar")]
        public async Task<IActionResult> Registrar([FromBody] UsuarioModel usuario)
        {
            RespuestaModel respuesta = await _seguridadManager.Registrar(usuario);
            return StatusCode(respuesta.Codigo, respuesta);
        }
    }
}