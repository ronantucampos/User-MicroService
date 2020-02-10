using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserService.Entities;
using UserService.Entities.Base;
using System.Security.Cryptography;
using MailKit.Net.Smtp;
using MimeKit;
using UserService.Business.View;

namespace UserService.Repository
{
    public class UsuarioRepository
    {
        private ContextBase _db;
        public UsuarioRepository()
        {
            _db = new ContextBase();
        }
        public List<Usuario> Listar()
        {
            return _db.Usuarios.ToList();
        }
        public List<Usuario> Listar(Pager pager)
        {
            pager.PageNumber = pager.PageNumber.GetValueOrDefault(0) == 0 ? 1 : pager.PageNumber;
            pager.PageSize = pager.PageSize.GetValueOrDefault(0) == 0 ? 25 : pager.PageSize;

            decimal qtdRegistros = _db.Usuarios.Count();
            pager.NumberOfRecord = (int)qtdRegistros;

            if (qtdRegistros == 0)
                pager.NumberOfPages = 1;
            else
                pager.NumberOfPages = (int)Math.Ceiling(qtdRegistros / Convert.ToDecimal(pager.PageSize.Value));

            int inicial = pager.PageSize.Value * (pager.PageNumber.Value - 1);

            return _db.Usuarios.Skip(inicial).Take(pager.PageSize.Value).ToList();
        }
        public Usuario Get(long id)
        {
            return _db.Usuarios.Where(w => w.Id == id).FirstOrDefault();
        }
        public Usuario Get(string login)
        {
            return _db.Usuarios.Where(w => w.Login == login).FirstOrDefault();
        }
        public bool Atualizar(Usuario usuario)
        {
            bool retorno = false;
            try
            {
                _db.Usuarios.Where(w => w.Id == usuario.Id)
                    .ToList()
                    .ForEach(u =>
                    {
                        u.Nome = usuario.Nome;
                        u.Email = usuario.Email;
                        u.DataAlteracao = usuario.DataAlteracao;
                        u.AlteradoPor = usuario.AlteradoPor;
                        retorno = true;
                    });
                _db.SaveChanges();
            }
            catch(Exception ex)
            {
                var dados = JsonConvert.SerializeObject(usuario);
                retorno = false;
                Logger.Debug(dados, ex);
                _db.Dispose();
                _db = new ContextBase();
            }
            return retorno;
        }
        public bool Criar(Usuario usuario)
        {
            bool retorno = false;

            try
            {
                var usuExiste = Get(usuario.Login);
                if (usuExiste == null || usuExiste.Id == 0)
                {
                    _db.Usuarios.Add(new Usuario()
                    {
                        Login = usuario.Login,
                        Nome = usuario.Nome,
                        DataCriacao = usuario.DataCriacao,
                        CriadoPor = usuario.CriadoPor,
                        Senha = usuario.Senha,
                        Email = usuario.Email
                    });
                    _db.SaveChanges();
                    retorno = true;
                }
                else
                {
                    Logger.Debug("Usuário: " + usuario.Login + ", já existe.");
                }
            }
            catch (Exception ex)
            {
                var dados = JsonConvert.SerializeObject(usuario);
                retorno = false;
                Logger.Debug(dados, ex);
                _db.Dispose();
                _db = new ContextBase();
            }

            return retorno;
        }
        public string GerarToken(string login, string senhaCript)
        {
            string token = string.Empty;
            try
            {
                _db.Usuarios.Where(w => w.Login == login && w.Senha == senhaCript)
                    .ToList()
                    .ForEach(u =>
                    {
                        token = Guid.NewGuid().ToString();
                        u.Token = token;
                        u.ValidadeToken = DateTime.Now.AddHours(1);
                    });
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                var dados = "Usuário: " + login;
                token = string.Empty;
                Logger.Debug(dados, ex);
                _db.Dispose();
                _db = new ContextBase();
            }
            return token;
        }
        public bool RenovarToken(string token)
        {
            bool retorno = false;
            try
            {
                _db.Usuarios.Where(w => w.Token == token)
                .ToList()
                .ForEach(u => 
                {
                    u.ValidadeToken = DateTime.Now.AddHours(1);
                });
                _db.SaveChanges();
                retorno = true;
            }
            catch (Exception ex)
            {
                var dados = "Erro ao renovar Token, verifique com o suporte.";
                retorno = false;
                Logger.Debug(dados, ex);
                _db.Dispose();
                _db = new ContextBase();
            }
            return retorno;
        }
        public bool ValidarToken(string token)
        {
            bool retorno = false;

            try
            {
                _db.Usuarios.Where(w => w.Token == token)
                .ToList()
                .ForEach(u =>
                {
                    if (u.ValidadeToken.GetValueOrDefault(DateTime.Now.AddMinutes(-1)) >= DateTime.Now)
                        retorno = true;
                    else
                        retorno = false;
                });
                if (retorno)
                    retorno = RenovarToken(token);
            }
            catch (Exception ex)
            {
                var dados = "Erro ao validar Token, verifique com o suporte.";
                retorno = false;
                Logger.Debug(dados, ex);
                _db.Dispose();
                _db = new ContextBase();
            }

            return retorno;
        }
        public string CriptPwd(string password)
        {
            string criptPwd = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(password))
                {
                    using (var md5 = MD5.Create())
                    {
                        var input = Encoding.ASCII.GetBytes(password);
                        var hash = md5.ComputeHash(input);
                        var sb = new StringBuilder();
                        for (int i = 0; i < hash.Length; i++)
                        {
                            sb.Append(hash[i].ToString("X2"));
                        }
                        criptPwd = sb.ToString().ToLower();
                    }
                }
            }
            catch (Exception ex)
            {
                var dados = "Erro ao criptografar a senha, verifique com o suporte.";
                Logger.Debug(dados, ex);
            }
            return criptPwd;
        }
        public void EnviarEmail(string nome, string email, string senha)
        {
            try
            {
                string emailEnvio = "testeCadastro@gmail.com";
                string senhaEnvio = "testeCadastro";
                var mensagem = new MimeMessage();
                mensagem.From.Add(new MailboxAddress("Não responder este e-mail", emailEnvio));
                mensagem.To.Add(new MailboxAddress(nome, email));
                mensagem.Subject = "Cadastro de usuários";
                mensagem.Body = new TextPart("plain")
                {
                    Text = "Senha: " + senha
                };
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate(emailEnvio, senhaEnvio);
                    client.Send(mensagem);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                var dados = "Erro ao enviar e-mail, verifique com o suporte.";
                Logger.Debug(dados, ex);
            }
        }
        public string TrocarSenha(Expression<Func<Usuario, bool>> condicao, string novaSenha, long? idUsuario)
        {
            string retorno = string.Empty;
            
            try
            {
                string novaSenhaCript = CriptPwd(novaSenha);

                var usuario = _db.Usuarios.Where(condicao);
                
                if (usuario != null)
                {
                    if (usuario.Count() > 0)
                    {
                        if (usuario.Count() == 1)
                        {
                            usuario.ToList()
                                .ForEach(u =>
                                {
                                    u.Senha = novaSenhaCript;
                                    u.DataAlteracao = DateTime.Now;
                                    u.AlteradoPor = idUsuario == null ? u.Id : idUsuario.Value;

                                    EnviarEmail(u.Nome, u.Email, novaSenha);
                                });
                            _db.SaveChanges();
                        }
                        else
                        {
                            retorno = "Os filtros informados resultaram e vários usuários, por favor informe mais filtros para garantir a troca da senha.";
                        }
                    }
                    else
                    {
                        retorno = "Usuário ou senha atual incorretos, verifique os filtros.";
                    }
                }
                else
                {
                    retorno = "Usuário não encontrado, verifique os filtros.";
                }
            }
            catch (Exception ex)
            {
                retorno = "Ocorreu erro ao trocar a senha, verifique com o suporte.";
                Logger.Debug(retorno, ex);
                _db.Dispose();
                _db = new ContextBase();
            }

            return retorno;
        }
        public string Deletar(long id)
        {
            string retorno = string.Empty;
            try
            {
                var usuario = _db.Usuarios.Where(w => w.Id == id);
                if (usuario != null)
                {
                    if (usuario.Count() > 0)
                    {
                        _db.Usuarios.RemoveRange(usuario.ToList());
                        _db.SaveChanges();
                    }
                    else
                    {
                        retorno = "Usuário não encontrado, não foi possível deletar registro.";
                    }
                }
                else
                {
                    retorno = "Id de Usuário não é válido, não foi possível deletar registro.";
                }
            }
            catch (Exception ex)
            {
                retorno = "Ocorreu erro ao deletar usuário" + id.ToString() + ", verifique com o suporte.";
                Logger.Debug(retorno, ex);
                _db.Dispose();
                _db = new ContextBase();
            }
            return retorno;
        }
    }
}
