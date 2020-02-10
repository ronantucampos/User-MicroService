using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserService.Business;
using AutoMapper;
using UserService.Business.View;

namespace UserService.Controllers
{
    [Route("cadastro/usuario")]
    public class UsuariosController : Controller
    {
        private UsuarioBusiness _usuario;
        public UsuariosController(IMapper mapper)
        {
            _usuario = new UsuarioBusiness(mapper);
        }
        //Listar todos usuários
        [HttpGet]
        public object Get()
        {
            return _usuario.Listar();
        }
        [HttpGet("listar")]
        public object GetPage(Pager pager)
        {
            return _usuario.Listar(pager);
        }
        //Listar usario
        [HttpGet("{id}")]
        public object GetById(long id)
        {
            return _usuario.Listar(id);
        }
        //Listar usario
        [HttpGet("{login}/busca")]
        public object GetByLogin(string login)
        {
            return _usuario.Listar(login);
        }
        // POST cadastro/usuario
        [HttpPost]
        public object Post([FromBody]UsuarioView usuario)
        {
            return _usuario.Gravar(usuario);
        }
        // POST cadastro/usuario
        [HttpPost("trocarsenha")]
        public object Post([FromBody]UsuarioFilter filtro)
        {
            return _usuario.TrocarSenha(filtro);
        }
        // PUT cadastro/usuario/5
        [HttpPost("acesso")]
        public object LiberarAcesso([FromBody]LoginView acesso)
        {
            return _usuario.Acesso(acesso.Login, acesso.Senha);
        }
        //valida o acesso
        [HttpGet("validaacesso")]
        public object ValidaAcesso([FromBody]string token)
        {
            return _usuario.ValidarToken(token);
        }
        // DELETE cadastro/usuario/5
        [HttpDelete("{id}")]
        public object Delete(long id)
        {
            return _usuario.Deletar(id);
        }
    }
}
