using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Articulo.Dominio
{
    public interface IArticuloRepository
    {

        Task<int> Create(ArticuloModel articulo, ArticuloTraceModel trace);
        Task<ArticuloModel?> Get(int codigo);
        Task<List<ArticuloModel>> Get();
        Task<int> Update(ArticuloModel articulo, ArticuloTraceModel trace);
        Task<int> Delete(int id, ArticuloTraceModel trace);
        Task<List<ArticuloModel>> Reporte();

    }
}
