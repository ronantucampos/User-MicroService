using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserService.Business.View;
using UserService.Entities;
using UserService.Entities.Base;
using UserService.Repository;

namespace UserService.Business
{
    public class UsuarioBusiness
    {
        private IMapper _mapper;
        private UsuarioRepository _repository;
        public UsuarioBusiness(IMapper mapper)
        {
            _mapper = mapper;
            _repository = new UsuarioRepository();
        }

        public string Listar()
        {
            return JsonConvert.SerializeObject(_mapper.Map<List<UsuarioView>>(_repository.Listar()));
        }
        public string Listar(Pager pager)
        {
            var viewResult = _mapper.Map<List<UsuarioView>>(_repository.Listar(pager));
            var listResult = new ListResultView<UsuarioView>()
            {
                PageNumber = pager.PageNumber,
                NumberOfRecord = pager.NumberOfRecord,
                NumberOfPages = pager.NumberOfPages,
                PageSize = pager.PageSize,
                List = viewResult
            };

            return JsonConvert.SerializeObject(listResult);
        }
        public string Listar(long id)
        {
            return JsonConvert.SerializeObject(_mapper.Map<UsuarioView>(_repository.Get(id)));
        }
        public string Listar(string login)
        {
            return JsonConvert.SerializeObject(_mapper.Map<UsuarioView>(_repository.Get(login)));
        }
        private string CriptPwd(string password)
        {
            return _repository.CriptPwd(password);
        }
        public ActionResult Gravar(UsuarioView usuario)
        {
            ActionResult retorno = new BadRequestResult();

            if (usuario.Id == 0)
            {
                var senha = CriptPwd(DateTime.Now.ToString("HHmiss"));
                if (_repository.Criar(new Usuario()
                {
                    Login = usuario.Login,
                    Nome = usuario.Nome,
                    DataCriacao = DateTime.Now,
                    CriadoPor = usuario.idUsuarioAlter.GetValueOrDefault(0) == 0 ? 1 : usuario.idUsuarioAlter.Value,
                    Senha = senha,
                    Email = usuario.Email
                }))
                {
                    EnviarEmail(usuario.Nome, usuario.Email, senha);
                    retorno = new OkObjectResult(senha);
                }
                else
                {
                    retorno = new BadRequestObjectResult("Ocorreu erro ao criar os dados, verifique com o suporte.");
                }
            }
            else
            {
                if (_repository.Atualizar(new Usuario()
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    DataAlteracao = DateTime.Now,
                    AlteradoPor = usuario.idUsuarioAlter.GetValueOrDefault(0) == 0 ? 1 : usuario.idUsuarioAlter.Value
                }))
                {
                    retorno = new OkResult();
                }
                else
                {
                    retorno = new BadRequestObjectResult("Ocorreu erro ao atualizar os dados, verifique com o suporte.");
                }
            }
            return retorno;
        }
        private void EnviarEmail(string nome, string email, string senha)
        {
            _repository.EnviarEmail(nome, email, senha);
        }
        public string Acesso(string login, string senha)
        {
            string senhaCript = string.Empty;
            if (senha.Length < 25)
                senhaCript = CriptPwd(senha);
            else
                senhaCript = senha;

            senhaCript = _repository.GerarToken(login, senhaCript);
            if (!string.IsNullOrEmpty(senhaCript))
                return senhaCript;
            else
                return "Falha ao realizar o login, usuário ou senha inválido.";
        }
        public ActionResult ValidarToken(string token)
        {
            if (_repository.ValidarToken(token))
            {
                return new OkResult();
            }
            return new BadRequestObjectResult("Token inválido.");
        }
        public ActionResult TrocarSenha(UsuarioFilter filtro)
        {
            if (!string.IsNullOrEmpty(filtro.senhaAnterior) &&
                !string.IsNullOrEmpty(filtro.senhaAtual) &&
                filtro.senhaAnterior.Trim() != string.Empty &&
                filtro.senhaAtual.Trim() != string.Empty &&
                (!string.IsNullOrEmpty(filtro.Login) ||
                 filtro.Id != null ||
                 !string.IsNullOrEmpty(filtro.Email)))
            {
                if (filtro.senhaAnterior != filtro.senhaAtual)
                {
                    if (ValidarSenha(filtro.senhaAtual))
                    {
                        string senhaCript = string.Empty;
                        if (filtro.senhaAnterior.Length < 25)
                            senhaCript = CriptPwd(filtro.senhaAnterior);
                        else
                            senhaCript = filtro.senhaAnterior;

                        Expression<Func<Usuario, bool>> condicao;

                        if (!string.IsNullOrEmpty(filtro.Login) &&
                            filtro.Id.GetValueOrDefault(0) != 0 &&
                            !string.IsNullOrEmpty(filtro.Email))
                            condicao = s => s.Senha == senhaCript && s.Login == filtro.Login && s.Id == filtro.Id && s.Email == filtro.Email;
                        else
                        if (!string.IsNullOrEmpty(filtro.Login) &&
                            filtro.Id.GetValueOrDefault(0) != 0 &&
                            string.IsNullOrEmpty(filtro.Email))
                            condicao = s => s.Senha == senhaCript && s.Login == filtro.Login && s.Id == filtro.Id;
                        else
                        if (!string.IsNullOrEmpty(filtro.Login) &&
                            filtro.Id.GetValueOrDefault(0) == 0 &&
                            string.IsNullOrEmpty(filtro.Email))
                            condicao = s => s.Senha == senhaCript && s.Login == filtro.Login;
                        else
                        if (string.IsNullOrEmpty(filtro.Login) &&
                            filtro.Id.GetValueOrDefault(0) != 0 &&
                            !string.IsNullOrEmpty(filtro.Email))
                            condicao = s => s.Senha == senhaCript && s.Id == filtro.Id && s.Email == filtro.Email;
                        else
                        if (string.IsNullOrEmpty(filtro.Login) &&
                            filtro.Id.GetValueOrDefault(0) == 0 &&
                            !string.IsNullOrEmpty(filtro.Email))
                            condicao = s => s.Senha == senhaCript && s.Email == filtro.Email;
                        else
                        if (string.IsNullOrEmpty(filtro.Login) &&
                            filtro.Id.GetValueOrDefault(0) != 0 &&
                            string.IsNullOrEmpty(filtro.Email))
                            condicao = s => s.Senha == senhaCript && s.Id == filtro.Id;
                        else
                            condicao = s => s.Senha == senhaCript;


                        var retorno = _repository.TrocarSenha(condicao, filtro.senhaAtual, filtro.idUsuarioAlter);

                        if (string.IsNullOrEmpty(retorno))
                            return new OkObjectResult("Senha alterada com sucesso.");
                        else
                            return new NotFoundObjectResult(retorno);
                    }
                    return new BadRequestObjectResult("A senha deve conter números, letras maiúsculas e minúsculas.");
                }
                return new BadRequestObjectResult("A senha não pode ser a mesma da anterior.");
            }
            return new BadRequestObjectResult("Obrigatório informar a senha anterior e atual, e um dos dados do usuário.");
        }
        private bool ValidarSenha(string senha)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasMinimum8Chars = new Regex(@".{3,15}");

            return hasNumber.IsMatch(senha) && hasUpperChar.IsMatch(senha) && hasLowerChar.IsMatch(senha) && hasMinimum8Chars.IsMatch(senha);
        }
        public ActionResult Deletar(long id)
        {
            var retorno = _repository.Deletar(id);

            if (string.IsNullOrEmpty(retorno))
                return new OkObjectResult("Usuário excluído com sucesso.");
            else
                return new NotFoundObjectResult(retorno);
        }
    }
}
