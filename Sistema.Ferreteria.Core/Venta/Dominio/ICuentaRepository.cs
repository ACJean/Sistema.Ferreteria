using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Venta.Dominio
{
    public interface ICuentaRepository
    {

        Task<int> Save(CuentaModel cuenta);
        Task<List<CuentaModel>> Get(FiltroCuentaModel filtro);

    }
}
