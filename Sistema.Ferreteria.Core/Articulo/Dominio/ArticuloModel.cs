using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Articulo.Dominio
{
    public class ArticuloModel
    {

        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public int Material { get; set; }
        public string Durabilidad { get; set; }
        public decimal Peso { get; set; }
        public string Tamanio { get; set; }
        public decimal Precio { get; set; }
        public short Stock { get; set; }
        public short? Estado { get; set; }
        public List<ArticuloImagenModel> Imagenes { get; set; }
        [JsonIgnore]
        public List<ArticuloTraceModel> TrazabilidadArticulo { get; set; }

        public ArticuloModel()
        {
            Imagenes = new List<ArticuloImagenModel>();
            TrazabilidadArticulo = new List<ArticuloTraceModel>();
        }

    }
}
