using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UserService.Entities.Base
{
    public class ContextBase : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Password=sa123;Persist Security Info=True;User ID=sa;Initial Catalog=Cadastro;Data Source=(localdb)\\MSSQLLocalDB");
        }
    }
}
