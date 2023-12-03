using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sistema.Ferreteria.Core.Articulo.Aplicacion;
using Sistema.Ferreteria.Core.Cliente.Dominio;
using Sistema.Ferreteria.Core.Compartido.Dominio;

namespace Sistema.Ferreteria.Core.Cliente.Aplicacion
{
    public class ClienteManager
    {

        private readonly ILogger<ArticulosManager> _logger;
        private readonly IConfiguration _config;
        private readonly IClienteRepository _clienteRepository;

        public ClienteManager(ILogger<ArticulosManager> logger, IConfiguration config, IClienteRepository clienteRepository)
        {
            _logger = logger;
            _config = config;
            _clienteRepository = clienteRepository;
        }

        public async Task<RespuestaModel> Actualizar(ClienteModel cliente)
        {
            RespuestaModel respuesta = new();
            try
            {

                int filasAfectadas = await _clienteRepository.Update(cliente);

                if (filasAfectadas <= 0)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se actualizó el cliente, intentelo de nuevo.";
                    return respuesta;
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Cliente actualizado.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Baja(string cedula)
        {
            RespuestaModel respuesta = new();
            try
            {
                ClienteModel? cliente = await _clienteRepository.Get(cedula);

                if (cliente == null)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se encontró el cliente";
                    respuesta.Datos = null;
                    return respuesta;
                }

                cliente.Estado = 2;

                int filasAfectadas = await _clienteRepository.Update(cliente);

                if (filasAfectadas <= 0)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se pudo dar de baja al cliente, intentelo de nuevo.";
                    return respuesta;
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Cliente dado de baja.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Crear(ClienteModel cliente)
        {
            RespuestaModel respuesta = new();
            try
            {
                int clienteId = await _clienteRepository.Create(cliente);
                cliente.Id = clienteId;

                respuesta.Codigo = 201;
                respuesta.Mensaje = "Cliente creado.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Eliminar(string cedula)
        {
            RespuestaModel respuesta = new();
            try
            {
                int filasAfectadas = await _clienteRepository.Delete(cedula);

                if (filasAfectadas <= 0)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se eliminó el cliente, intentelo de nuevo.";
                    return respuesta;
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Cliente eliminado.";
            }
            catch (Exception ex)
            {
                respuesta.Codigo = 500;
                respuesta.Mensaje = ex.Message;
                _logger.LogError(new EventId(1), ex, ex.Message);
            }
            return respuesta;
        }

        public async Task<RespuestaModel> Obtener(string cedula)
        {
            RespuestaModel respuesta = new();
            try
            {

                ClienteModel? cliente = await _clienteRepository.Get(cedula);

                if (cliente == null)
                {
                    respuesta.Codigo = 200;
                    respuesta.Mensaje = "No se encontró el cliente.";
                    respuesta.Datos = null;
                    return respuesta;
                }

                respuesta.Codigo = 200;
                respuesta.Mensaje = "Cliente encontrado.";
                respuesta.Datos = cliente;
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
