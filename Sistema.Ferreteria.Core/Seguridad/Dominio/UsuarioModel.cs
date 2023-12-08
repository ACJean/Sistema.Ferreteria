using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema.Ferreteria.Core.Seguridad.Dominio
{
    public class UsuarioModel
    {

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; }
        public string Cedula { get; set; }

    }
}
