using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UserService.Controllers;
using UserService.DomainToView;
using Xunit;

namespace UserService.UnitTest
{
    public class UsuarioTests
    {
        private UsuariosController userTest;
        public UsuarioTests()
        {
            var configAutoMapper = new MapperConfiguration(c => c.AddProfile(new AutoMapperProfile()));
            var mapper = configAutoMapper.CreateMapper();
            userTest = new UsuariosController(mapper);
        }

        [Fact]
        public void Cria_Consulta_Deleta_Usuario_ReturnsTrue()
        {
            #region Cria Usuário
            var retPost = (OkObjectResult)userTest.Post(new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste1",
                Nome = "Usuário de teste 1",
                Email = "teste1@gmail.com"
            });

            Assert.True(retPost.StatusCode == 200);
            #endregion

            #region Consulta Todos
            var retGetAll = userTest.Get();
            var usuListView = JsonConvert.DeserializeObject<List<Business.View.UsuarioView>>(retGetAll.ToString());

            Assert.True(usuListView.Count > 0);
            #endregion

            #region Consulta Usuário
            var retGet = userTest.GetByLogin("UsuarioTeste1");
            var usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            Assert.True(usuView?.Id > 0);
            #endregion

            #region Delete Id inválido Usuário
            var retDel = userTest.Delete(0);
            bool condVal = false;
            try
            {
                var valOk = (NotFoundObjectResult)retDel;
                condVal = valOk.StatusCode == 404;
            }
            catch { condVal = false; }

            Assert.True(condVal);
            #endregion

            #region Delete Usuário
            if (usuView?.Id != 0)
            {
                retDel = userTest.Delete(usuView.Id);
                condVal = false;
                try
                {
                    var valOk = (OkObjectResult)retDel;
                    condVal = valOk.StatusCode == 200;
                }
                catch { condVal = false; }

                Assert.True(condVal);
            }
            #endregion
        }
        [Fact]
        public void ListaVarios_Usuario_ReturnsTrue()
        {
            #region Cria Usuário
            ActionResult retCriacao;
            for (var i = 20; i < 30; i++)
            {
                retCriacao = (ActionResult)userTest.Post(new Business.View.UsuarioView()
                {
                    Login = "UsuarioTeste" + i,
                    Nome = "Usuário de teste " + i,
                    Email = "teste" + i + "@gmail.com"
                });
            }
            #endregion
            #region Lista sem paginar
            var result = userTest.GetPage(new Business.View.Pager());
            var retConsulta = JsonConvert.DeserializeObject<Business.View.ListResultView<Business.View.UsuarioView>>(result.ToString());

            Assert.True(retConsulta.NumberOfPages.HasValue);
            Assert.True(retConsulta.NumberOfRecord.HasValue);
            Assert.True(retConsulta.PageNumber.HasValue);
            Assert.True(retConsulta.PageSize.HasValue);
            Assert.True(retConsulta.List.Count > 0);
            #endregion

            #region Lista paginado
            result = userTest.GetPage(new Business.View.Pager()
            {
                PageSize = 3,
                PageNumber = 2
            });
            retConsulta = JsonConvert.DeserializeObject<Business.View.ListResultView<Business.View.UsuarioView>>(result.ToString());

            Assert.True(retConsulta.NumberOfPages.HasValue);
            Assert.True(retConsulta.NumberOfRecord.HasValue);
            Assert.True(retConsulta.PageNumber.HasValue);
            Assert.True(retConsulta.PageSize.HasValue);
            Assert.True(retConsulta.List.Count > 0);
            #endregion

            #region Busca e limpa base
            for (var i = 20; i < 30; i++)
            {
                var retGet = userTest.GetByLogin("UsuarioTeste" + i);
                var usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

                if (usuView?.Id != 0)
                {
                    var retDel = userTest.Delete(usuView.Id);
                }
            }
            #endregion
        }
        [Fact]
        public void Cria_Usuario_Duplicado_ReturnsTrue()
        {
            #region Cria primeiro Usuario
            var retPost = (OkObjectResult)userTest.Post(new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste2",
                Nome = "Usuário de teste 2",
                Email = "teste2@gmail.com"
            });
            #endregion

            #region Cria segundo Usuario e Valida
            var retPostDup = (BadRequestObjectResult)userTest.Post(new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste2",
                Nome = "Usuário de teste 2",
                Email = "teste2@gmail.com"
            });

            Assert.True(retPostDup.StatusCode == 400);
            #endregion

            #region Busca e limpa base
            var retGet = userTest.GetByLogin("UsuarioTeste2");
            var usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            if (usuView?.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
            #endregion
        }
        [Fact]
        public void Altera_Usuario_ReturnsTrue()
        {
            #region Cria Usuário
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste3",
                Nome = "Usuário de teste 3",
                Email = "teste3@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);
            #endregion

            #region Altera Usuário
            var retGet = userTest.GetByLogin("UsuarioTeste3");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            usuView.Nome = "Usuário de teste 4";
            usuView.Email = "teste4@gmail.com";

            var retPost2 = (OkResult)userTest.Post(usuView);

            Assert.True(retPost2.StatusCode == 200);
            #endregion

            #region Confirma alteração do usuário
            retGet = userTest.GetById(usuView.Id);
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            Assert.True(usuView.Nome == "Usuário de teste 4" && usuView.Email == "teste4@gmail.com");
            #endregion

            if (usuView.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }

            #region Tenta alterar Usuário que não existe mais
            usuView.Nome = "Usuário de teste 3";
            usuView.Email = "teste3@gmail.com";

            var retPost3 = (BadRequestObjectResult)userTest.Post(usuView);

            Assert.True(retPost3.StatusCode == 400 && retPost3.Value.ToString() == "Ocorreu erro ao atualizar os dados, verifique com o suporte.");
            #endregion
        }
        [Fact]
        public void AlteraSenha_Usuario_ReturnsTrue()
        {
            #region Cria Usuário para teste
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste5",
                Nome = "Usuário de teste 5",
                Email = "teste5@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);

            var retSenha = retPost.Value.ToString();
            #endregion

            #region Altera senha temporária gerada
            var retAltSenha = userTest.Post(new Business.View.UsuarioFilter()
            {
                Id = null,
                Login = "UsuarioTeste5",
                senhaAnterior = retSenha,
                senhaAtual = "Test5"
            });

            bool result = false;
            try
            {
                retPost = (OkObjectResult)retAltSenha;
                result = true;
            }
            catch { result = false; }

            Assert.True(result);
            #endregion

            #region Busca e limpa base
            var retGet = userTest.GetByLogin("UsuarioTeste5");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            if (usuView?.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
            #endregion
        }
        [Fact]
        public void SenhaInvalida_Usuario_ReturnsTrue()
        {
            #region Cria Usuário para teste
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste6",
                Nome = "Usuário de teste 6",
                Email = "teste6@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);

            var retSenha = retPost.Value.ToString();
            #endregion

            #region Altera senha temporária por senha inválida
            var retAltSenha = userTest.Post(new Business.View.UsuarioFilter()
            {
                Id = null,
                Login = "UsuarioTeste6",
                senhaAnterior = retSenha,
                senhaAtual = "test"
            });

            string result = string.Empty;
            try
            {
                var retErro = (BadRequestObjectResult)retAltSenha;
                result = retErro.Value.ToString();
            }
            catch { result = string.Empty; }

            Assert.True(result == "A senha deve conter números, letras maiúsculas e minúsculas.");
            #endregion

            #region Busca e limpa base
            var retGet = userTest.GetByLogin("UsuarioTeste6");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            if (usuView?.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
            #endregion
        }
        [Fact]
        public void SenhaRepetida_Usuario_ReturnsTrue()
        {
            #region Cria Usuário para teste
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste7",
                Nome = "Usuário de teste 7",
                Email = "teste7@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);

            var retSenha = retPost.Value.ToString();
            #endregion

            #region Altera senha por senha repetida
            var retAltSenha = userTest.Post(new Business.View.UsuarioFilter()
            {
                Id = null,
                Login = "UsuarioTeste7",
                senhaAnterior = retSenha,
                senhaAtual = retSenha
            });

            string result = string.Empty;
            try
            {
                var retErro = (BadRequestObjectResult)retAltSenha;
                result = retErro.Value.ToString();
            }
            catch { result = string.Empty; }

            Assert.True(result == "A senha não pode ser a mesma da anterior.");
            #endregion

            #region Busca e limpa base
            var retGet = userTest.GetByLogin("UsuarioTeste7");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            if (usuView?.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
            #endregion
        }
        [Fact]
        public void SenhaNaoInformada_Usuario_ReturnsTrue()
        {
            #region Cria Usuário para teste
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste8",
                Nome = "Usuário de teste 8",
                Email = "teste8@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);
            #endregion

            #region Altera senha por senha repetida
            var retAltSenha = userTest.Post(new Business.View.UsuarioFilter()
            {
                Id = null,
                Login = "UsuarioTeste8",
                senhaAnterior = string.Empty,
                senhaAtual = string.Empty
            });

            string result = string.Empty;
            try
            {
                var retErro = (BadRequestObjectResult)retAltSenha;
                result = retErro.Value.ToString();
            }
            catch { result = string.Empty; }

            Assert.True(result == "Obrigatório informar a senha anterior e atual, e um dos dados do usuário.");
            #endregion

            #region Busca e limpa base
            var retGet = userTest.GetByLogin("UsuarioTeste8");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            if (usuView?.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
            #endregion
        }
        [Fact]
        public void FiltrosAlteraSenha_Usuario_ReturnsTrue()
        {
            #region Cria Usuário para teste
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste9",
                Nome = "Usuário de teste 9",
                Email = "teste9@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);

            var retSenha = retPost.Value.ToString();
            #endregion

            #region Filtro pelo Login
            var retAltSenha = userTest.Post(new Business.View.UsuarioFilter()
            {
                Login = "UsuarioTeste9",
                senhaAnterior = retSenha,
                senhaAtual = "Test9"
            });
            retSenha = "Test9";

            bool result = false;
            try
            {
                retPost = (OkObjectResult)retAltSenha;
                result = true;
            }
            catch { result = false; }

            Assert.True(result);
            #endregion

            var retGet = userTest.GetByLogin("UsuarioTeste9");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            #region Filtro pelo Id
            retAltSenha = userTest.Post(new Business.View.UsuarioFilter()
            {
                Id = usuView.Id,
                senhaAnterior = retSenha,
                senhaAtual = "Test9Dois"
            });
            retSenha = "Test9Dois";

            result = false;
            try
            {
                retPost = (OkObjectResult)retAltSenha;
                result = true;
            }
            catch { result = false; }

            Assert.True(result);
            #endregion

            #region Filtro pelo Email
            retAltSenha = userTest.Post(new Business.View.UsuarioFilter()
            {
                Email = usuView.Email,
                senhaAnterior = retSenha,
                senhaAtual = "Test9Tres"
            });
            retSenha = "Test9Tres";

            result = false;
            try
            {
                retPost = (OkObjectResult)retAltSenha;
                result = true;
            }
            catch { result = false; }

            Assert.True(result);
            #endregion

            if (usuView.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
        }
        [Fact]
        public void AcessoSistema_Usuario_ReturnsTrue()
        {
            #region Cria Usuário para teste
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste10",
                Nome = "Usuário de teste 10",
                Email = "teste10@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);

            var retSenha = retPost.Value.ToString();
            #endregion

            #region Acesso com a senha válida
            var retAcesso = userTest.LiberarAcesso(new Business.View.LoginView()
            {
                Login = "UsuarioTeste10",
                Senha = retSenha
            });

            Assert.NotNull(retAcesso);
            Assert.DoesNotContain(retAcesso.ToString(), "Falha ao realizar o login, usuário ou senha inválido.");
            #endregion

            #region Acesso com a senha inválida
            retAcesso = userTest.LiberarAcesso(new Business.View.LoginView()
            {
                Login = "UsuarioTeste10",
                Senha = "SenhaInvalida10"
            });

            Assert.NotNull(retAcesso);
            Assert.Contains(retAcesso.ToString(), "Falha ao realizar o login, usuário ou senha inválido.");
            #endregion

            var retGet = userTest.GetByLogin("UsuarioTeste10");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            if (usuView.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
        }
        [Fact]
        public void ValidaAcessoToken_Usuario_ReturnsTrue()
        {
            #region Cria Usuário para teste
            var usuView = new Business.View.UsuarioView()
            {
                Login = "UsuarioTeste11",
                Nome = "Usuário de teste 11",
                Email = "teste11@gmail.com"
            };
            var retPost = (OkObjectResult)userTest.Post(usuView);

            var retSenha = retPost.Value.ToString();
            #endregion

            #region Captura Token acesso
            var retAcesso = userTest.LiberarAcesso(new Business.View.LoginView()
            {
                Login = "UsuarioTeste11",
                Senha = retSenha
            });
            #endregion

            #region Token Válido
            var retValita = (ActionResult)userTest.ValidaAcesso(retAcesso.ToString());
            bool valValido = false;
            try
            {
                var retConf = (OkResult)retValita;
                valValido = retConf.StatusCode == 200;
            }
            catch { valValido = false; }

            Assert.True(valValido);
            #endregion

            #region Token Inválido
            retValita = (ActionResult)userTest.ValidaAcesso("TesteDeAcessoInvalido");
            valValido = false;
            try
            {
                var retConf = (BadRequestObjectResult)retValita;
                valValido = retConf.Value.ToString() == "Token inválido.";
            }
            catch { valValido = false; }

            Assert.True(valValido);
            #endregion

            var retGet = userTest.GetByLogin("UsuarioTeste11");
            usuView = JsonConvert.DeserializeObject<Business.View.UsuarioView>(retGet.ToString());

            if (usuView.Id != 0)
            {
                var retDel = userTest.Delete(usuView.Id);
            }
        }
    }
}
