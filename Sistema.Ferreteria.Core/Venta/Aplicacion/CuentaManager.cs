using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sistema.Ferreteria.Core.Compartido.Dominio;
using Sistema.Ferreteria.Core.Venta.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Venta.Aplicacion
{
    public class CuentaManager
    {

        private readonly ILogger<CuentaManager> _logger;
        private readonly IConfiguration _config;
        private readonly ICuentaRepository _cuentaRepository;

        public CuentaManager(ILogger<CuentaManager> logger, IConfiguration config, ICuentaRepository cuentaRepository)
        {
            _logger = logger;
            _config = config;
            _cuentaRepository = cuentaRepository;
        }

        public async Task<RespuestaModel> Obtener(FiltroCuentaModel filtro)
        {
            RespuestaModel respuesta = new();
            try
            {
                List<CuentaModel> cuentas = await _cuentaRepository.Get(filtro);

                respuesta.Codigo = 200;
                respuesta.Mensaje = $"{cuentas.Count} cuentas encontradas.";
                respuesta.Datos = cuentas;
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Procesar(CuentaModel cuenta)
        {
            RespuestaModel respuesta = new();
            try
            {
                cuenta.FechaEmision = DateTime.Now;
                _logger.LogTrace("Cuenta nueva: {Cuenta}", cuenta);

                int cuentaId = await _cuentaRepository.Save(cuenta);

                respuesta.Codigo = 201;
                respuesta.Mensaje = $"Cuenta #{cuentaId} cerrada.";

                _logger.LogTrace(respuesta.Mensaje);
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
