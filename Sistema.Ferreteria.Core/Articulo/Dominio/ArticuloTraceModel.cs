using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Articulo.Dominio
{
    public class ArticuloTraceModel
    {

        public int Id { get; set; }
        public int ArticuloId { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public int UsuarioId { get; set; }

    }
}
