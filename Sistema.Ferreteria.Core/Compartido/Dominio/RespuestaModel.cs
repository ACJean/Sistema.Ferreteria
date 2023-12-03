using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Compartido.Dominio
{
    public class RespuestaModel
    {

        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public object? Datos { get; set; }

    }
}
