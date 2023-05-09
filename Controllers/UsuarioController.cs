using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using API.Models;
using API.Services.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("v1/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<dynamic> Authenticate([FromBody] UsuarioTab model)
        {
            var user = new UsuarioService().Login(model);

            if (user == null)
                return BadRequest(new { mensagem = "Usuário ou senha inválidos" });

            if (user.status == "Inativo")
                return BadRequest(new { mensagem = "Usuário inativo, contate o administrador." });

            bool gerenteBloqueado = false;

            if (user.gerente != "")
            {
                gerenteBloqueado = new UsuarioService().UsuarioBloqueado(model.gerente);

                if (gerenteBloqueado == true)
                {
                    return BadRequest(new { mensagem = "Usuário inativo, contate o administrador." });
                }
            }

            var access_token = Services.TokenService.GenerateToken(user);
            user.senha = "";

            return new
            {
                user,
                access_token
            };
        }

        [HttpPost("refresh_token")]
        [AllowAnonymous]
        public ActionResult<dynamic> Refresh_token([FromBody] UsuarioTab model)
        {
            var user = new UsuarioService().Refresh_token(model);

            if (user == null)
                return BadRequest(new { mensagem = "Usuário ou senha inválidos." });

            if (user.status == "Inativo")
                return BadRequest(new { mensagem = "Usuário inativo, contate o administrador." });

            bool gerenteBloqueado = false;

            if (user.gerente != "")
            {
                gerenteBloqueado = new UsuarioService().UsuarioBloqueado(model.gerente);

                if (gerenteBloqueado == true)
                {
                    return BadRequest(new { mensagem = "Usuário inativo, contate o administrador." });
                }
            }

            var access_token = Services.TokenService.GenerateToken(user);
            user.senha = "";

            return new
            {
                user,
                access_token
            };
        }

        [HttpPut]
        [Route("criar-usuario")]
        [Authorize(Roles = "Administrador, Gerente")]
        public string CriarUsuario([FromBody] UsuarioTab usuario)
        {
            return new UsuarioService().CriarUsuario(usuario);
        }

        [HttpPut]
        [Route("criar-apostador")]
        [AllowAnonymous]
        public string CriarApostador([FromBody] UsuarioTab usuario)
        {
            return new UsuarioService().CriarApostador(usuario);
        }

        [HttpPost]
        [Route("excluir-usuario")]
        [Authorize(Roles = "Administrador, Gerente")]
        public string ExcluirUsuario([FromBody] UsuarioTab usuario)
        {
            return new UsuarioService().ExcluirUsuario(usuario);
        }

        [HttpPost]
        [Route("editar-usuario")]
        [Authorize(Roles = "Administrador, Gerente")]
        public string EditarUsuario([FromBody] UsuarioTab usuario)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            return new UsuarioService().EditarUsuario(usuario, tipo.Value);
        }

        [HttpPost]
        [Route("alterar-status")]
        [Authorize(Roles = "Administrador, Gerente")]
        public string AlterarStatus([FromBody] UsuarioTab usuario)
        {
            return new UsuarioService().AlterarStatus(usuario);
        }

        [HttpPost]
        [Route("alterar-senha")]
        [Authorize(Roles = "Administrador, Gerente, Vendedor, Apostador")]
        public string AlterarSenha([FromBody] UsuarioTab usuario)
        {
            return new UsuarioService().AlterarSenha(usuario);
        }

        [HttpGet]
        [Route("vendedores")]
        [Authorize(Roles = "Administrador, Gerente")]
        public List<UsuarioTab> GetVendedores()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            string id = User.Claims.First(c => c.Type == "Id").Value;

            return new UsuarioService().ListaVendedores(id, tipo.Value);
        }

        [HttpGet]
        [Route("vendedores-ativos")]
        [Authorize(Roles = "Administrador")]
        public List<UsuarioTab> GetVendedoresAtivos()
        {
            return new UsuarioService().ListaVendedoresAtivos();
        }

        [HttpGet]
        [Route("gerentes")]
        [Authorize(Roles = "Administrador")]
        public List<UsuarioTab> GetGerentes()
        {
            return new UsuarioService().ListaGerentes();
        }

        [HttpGet]
        [Route("transferir-gerente")]
        [AllowAnonymous]
        public string Transferir()
        {
            return new UsuarioService().transferir();
        }

        [HttpGet]
        [Route("transferir-vendedor")]
        [AllowAnonymous]
        public string TransferirVendedor()
        {
            return new UsuarioService().transferirVendedor();
        }
    }
}