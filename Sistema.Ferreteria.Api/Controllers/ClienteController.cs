using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Ferreteria.Core.Cliente.Aplicacion;
using Sistema.Ferreteria.Core.Cliente.Dominio;
using Sistema.Ferreteria.Core.Compartido.Dominio;

namespace Sistema.Ferreteria.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("clientes")]
    public class ClienteController : ControllerBase
    {

        private readonly ClienteManager _clienteManager;

        public ClienteController(ClienteManager clienteManager)
        {
            _clienteManager = clienteManager;
        }

        [HttpGet]
        [Route("{cedula}")]
        public async Task<IActionResult> Cliente([FromRoute] string cedula)
        {
            RespuestaModel respuesta = await _clienteManager.Obtener(cedula);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> Crear([FromBody] ClienteModel cliente)
        {
            RespuestaModel respuesta = await _clienteManager.Crear(cliente);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpPut]
        [Route("actualizar/{cedula}")]
        public async Task<IActionResult> Actualizar([FromBody] ClienteModel cliente, [FromRoute] string cedula)
        {
            cliente.Cedula = cedula;
            RespuestaModel respuesta = await _clienteManager.Actualizar(cliente);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpDelete]
        [Route("eliminar/{cedula}")]
        public async Task<IActionResult> Eliminar([FromRoute] string cedula)
        {
            RespuestaModel respuesta = await _clienteManager.Eliminar(cedula);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpPut]
        [Route("baja/{cedula}")]
        public async Task<IActionResult> Baja([FromRoute] string cedula)
        {
            RespuestaModel respuesta = await _clienteManager.Baja(cedula);
            return StatusCode(respuesta.Codigo, respuesta);
        }

    }
}
