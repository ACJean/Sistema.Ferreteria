using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Cliente.Dominio
{
    public interface IClienteRepository
    {

        Task<int> Create(ClienteModel cliente);
        Task<ClienteModel?> Get(string cedula);
        Task<int> Update(ClienteModel cliente);
        Task<int> Delete(string cedula);

    }
}
