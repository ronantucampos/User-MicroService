using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserService.Repository;

namespace UserService.UnitTestes
{
    [TestClass]
    public class UsuarioTests
    {
        [TestMethod]
        public void Cria_Deleta_Usuario_ReturnOk()
        {
            var usuarioRepository = new UsuarioRepository();

            var result = usuarioRepository.Criar(new Entities.Usuario()
            {
                CriadoPor = 1,
                DataCriacao = DateTime.Now,
                Login = "testeCriaUsu",
                Nome = "Criacao de usuario de teste",
                Email = "teste@gmail.com",
                Senha = "senhateste"
            });

            Assert.IsTrue(result);

            var usuCriado = usuarioRepository.Get("testeCriaUsu");

            var retorno = usuarioRepository.Deletar(usuCriado.Id);

            Assert.IsNull(retorno);
        }
    }
}
