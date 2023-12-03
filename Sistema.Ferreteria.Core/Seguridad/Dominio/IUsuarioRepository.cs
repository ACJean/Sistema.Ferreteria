using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Seguridad.Dominio
{
    public interface IUsuarioRepository
    {

        Task<UsuarioModel?> Get(string usuario, string clave);
        Task<int> Register(UsuarioModel usuario);

    }
}
