using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Business.View
{
    public class UsuarioView
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public long? idUsuarioAlter { get; set; }
    }
}
