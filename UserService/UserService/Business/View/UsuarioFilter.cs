using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Business.View
{
    public class UsuarioFilter
    {
        public long? Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string senhaAnterior { get; set; }
        public string senhaAtual { get; set; }
        public long? idUsuarioAlter { get; set; }
    }
}
