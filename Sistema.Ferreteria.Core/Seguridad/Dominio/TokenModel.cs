using Sistema.Ferreteria.Core.Compartido.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Ferreteria.Core.Seguridad.Dominio
{
    public class TokenModel : RespuestaModel
    {

        public string Token { get; }

        public TokenModel(string token)
        {
            Token = token;
        }

    }
}
