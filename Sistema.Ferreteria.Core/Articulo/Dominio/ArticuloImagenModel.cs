using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Articulo.Dominio
{
    public class ArticuloImagenModel
    {

        public int? Id { get; set; }
        public int? ArticuloId { get; set; }
        [JsonIgnore]
        public byte[]? Imagen { get; set; }
        public string ImagenBase64 { get; set; }

    }
}
