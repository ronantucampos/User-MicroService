using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UserService.Entities.Base;

namespace UserService.Entities
{
    public class Usuario : EntityBase
    {
        public string Login { get; set; }
        public string Senha { get; set; }
        public string Nome { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime? ValidadeToken { get; set; }
    }
}
