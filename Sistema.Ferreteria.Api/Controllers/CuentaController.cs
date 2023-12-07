using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Ferreteria.Core.Compartido.Dominio;
using Sistema.Ferreteria.Core.Venta.Aplicacion;
using Sistema.Ferreteria.Core.Venta.Dominio;
using System.Security.Claims;

namespace Sistema.Ferreteria.Api.Controllers
{

    [ApiController]
    [Route("cuentas")]
    public class CuentaController : ControllerBase
    {

        private readonly CuentaManager _cuentaManager;

        public CuentaController(CuentaManager cuentaManager)
        {
            _cuentaManager = cuentaManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("procesar")]
        public async Task<IActionResult> Procesar([FromBody] CuentaModel cuenta)
        {
            RespuestaModel respuesta = await _cuentaManager.Procesar(cuenta);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Cuentas([FromBody] FiltroCuentaModel filtro)
        {
            RespuestaModel respuesta = await _cuentaManager.Obtener(filtro);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CuentasCliente()
        {
            int usuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue("id"));
            RespuestaModel respuesta = await _cuentaManager.Obtener(usuarioId);
            return StatusCode(respuesta.Codigo, respuesta);
        }

    }
}
