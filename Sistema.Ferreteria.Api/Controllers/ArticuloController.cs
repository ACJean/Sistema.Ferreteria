using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Ferreteria.Core.Articulo.Aplicacion;
using Sistema.Ferreteria.Core.Articulo.Dominio;
using Sistema.Ferreteria.Core.Compartido.Dominio;
using System.Data;
using System.Security.Claims;

namespace Sistema.Ferreteria.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("articulos")]
    public class ArticuloController : ControllerBase
    {

        private readonly ArticulosManager _articuloManager;

        public ArticuloController(ArticulosManager articuloManager)
        {
            _articuloManager = articuloManager;
        }

        [HttpGet]
        public async Task<IActionResult> Articulos()
        {
            RespuestaModel respuesta = await _articuloManager.Obtener();
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpGet]
        [Route("{codigo}")]
        public async Task<IActionResult> Articulo([FromRoute] int codigo)
        {
            RespuestaModel respuesta = await _articuloManager.Obtener(codigo);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IActionResult> Crear([FromBody] ArticuloModel articulo)
        {
            int usuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue("id"));
            RespuestaModel respuesta = await _articuloManager.Crear(articulo, usuarioId);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public async Task<IActionResult> Actualizar([FromBody] ArticuloModel articulo, [FromRoute] int id)
        {
            int usuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue("id"));
            articulo.Id = id;
            RespuestaModel respuesta = await _articuloManager.Actualizar(articulo, usuarioId);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public async Task<IActionResult> Eliminar([FromRoute] int id)
        {
            int usuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue("id"));
            RespuestaModel respuesta = await _articuloManager.Eliminar(id, usuarioId);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpPut]
        [Route("baja/{codigo}")]
        public async Task<IActionResult> Baja([FromRoute] int codigo)
        {
            int usuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue("id"));           
            RespuestaModel respuesta = await _articuloManager.Baja(codigo, usuarioId);
            return StatusCode(respuesta.Codigo, respuesta);
        }

        [HttpGet]
        [Route("reporte")]
        public async Task<IActionResult> ReporteArticulo()
        {
            RespuestaModel respuesta = await _articuloManager.Reporte();
            return StatusCode(respuesta.Codigo, respuesta);
        }

    }
}
