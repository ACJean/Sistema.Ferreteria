using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Ferreteria.Core.Compartido.Dominio;
using Sistema.Ferreteria.Core.Venta.Aplicacion;
using Sistema.Ferreteria.Core.Venta.Dominio;

namespace Sistema.Ferreteria.Api.Controllers
{

    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("cuentas")]
    public class CuentaController : ControllerBase
    {

        private readonly CuentaManager _cuentaManager;

        public CuentaController(CuentaManager cuentaManager)
        {
            _cuentaManager = cuentaManager;
        }

        [HttpPost]
        [Route("procesar")]
        public async Task<IActionResult> Procesar([FromBody] CuentaModel cuenta)
        {
            RespuestaModel respuesta = await _cuentaManager.Procesar(cuenta);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpPost]
        public async Task<IActionResult> Cuentas([FromBody] FiltroCuentaModel filtro)
        {
            RespuestaModel respuesta = await _cuentaManager.Obtener(filtro);
            return StatusCode(respuesta.Codigo, respuesta);
        }

    }
}
