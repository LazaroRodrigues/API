using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models;
using API.Services.Bilhete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [Route("v1/bilhete")]
    [ApiController]
    public class BilheteController : ControllerBase
    {
        [HttpPost]
        [Route("bilhetes")]
        [Authorize(Roles = "Administrador, Gerente, Vendedor, Apostador")]
        public List<BilheteTab> GetBilhetes([FromBody] SorteioTab rifa)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            string Id = User.Claims.First(c => c.Type == "Id").Value;

            if (tipo.Value == "Administrador")
            {
                return new BilheteService().ListaBilhetes(rifa);
            }
            else if (tipo.Value == "Gerente")
            {
                return new BilheteService().ListaBilhetesGerente(rifa, Id);
            }
            else
            {
                return new BilheteService().ListaBilhetesVendedor(rifa, Id);
            }
        }

        [HttpPost]
        [Route("bilhetes-vencedores")]
        [Authorize(Roles = "Administrador, Gerente, Vendedor, Apostador")]
        public List<BilheteTab> GetBilhetesVencedores([FromBody] AuxGerarPalpite json)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            string Id = User.Claims.First(c => c.Type == "Id").Value;

            if (tipo.Value != "Administrador")
            {
                json.gerente = tipo.Value == "Vendedor" ? json.gerente : tipo.Value == "Gerente" ? Id : json.gerente;
            }

            if (tipo.Value == "Administrador")
            {
                return new BilheteService().ListaBilhetesVencedoresAdmin(json);
            }
            else
            {
                return new BilheteService().ListaBilhetesVencedores(json);
            }
        }

        [HttpPost]
        [Route("gerar-palpites")]
        [AllowAnonymous]
        public List<PalpitesTab> GerarPalpites([FromBody] AuxGerarPalpite json)
        {
            return new BilheteService().gerarPalpites(json.quantidade, json.id_sorteio, json.gerente);
        }

        [HttpPost]
        [Route("bilhetes-telefone")]
        [AllowAnonymous]
        public List<BilheteTab> GetPalpitesPorTelefone([FromBody] BilheteTab bilhete)
        {
            return new BilheteService().PalpitesPorTelefone(bilhete.origem, bilhete.id_sorteio);
        }

        [HttpPost]
        [Route("bilhete-id")]
        [AllowAnonymous]
        public BilheteTab GetBilhetePorId([FromBody] BilheteTab bilhete)
        {
            return new BilheteService().BilhetePorId(bilhete.id);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("bilhete-codigo")]
        [Authorize(Roles = "Administrador, Gerente, Vendedor, Cliente, Apostador")]
        public BilheteTab GetBilhetePorCodigo([FromBody] BilheteTab bilhete)
        {
            return new BilheteService().BilhetePorCodigo(bilhete.codigo);
        }

        [HttpPost]
        [Route("palpites")]
        [Authorize(Roles = "Administrador, Gerente, Vendedor, Cliente, Apostador")]
        public List<PalpitesTab> GetPalpites([FromBody] SorteioTab rifa)
        {
            return new BilheteService().Palpites(rifa.id);
        }

        [HttpPut]
        [Route("criar-bilhete")]
        [Authorize(Roles = "Administrador, Vendedor")]
        public BilheteTab CriarBolao([FromBody] BilheteTab bilhete)
        {
            return new BilheteService().CriarBilhete(bilhete);
        }

        [HttpPut]
        [Route("criar-bilhete-apostador")]
        [Authorize(Roles = "Apostador")]
        public async Task<BilheteTab> CriarBilheteApostadorAsync([FromBody] BilheteTab bilhete)
        {
            return await new BilheteService().CriarBilheteApostador(bilhete);
        }

        [HttpPost]
        [Route("cancelar-bilhete")]
        [Authorize(Roles = "Administrador")]
        public string CancelarBilhete([FromBody] BilheteTab bilhete)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            string Id = User.Claims.First(c => c.Type == "Id").Value;
            bilhete.cancelado_por = User.Claims.First(c => c.Type == "Usuario").Value;
            return new BilheteService().CancelarBilhete(bilhete);
        }

        [HttpPost]
        [Route("atualizar-resultados")]
        [Authorize(Roles = "Administrador")]
        public string AtualizarResultados([FromBody] AuxResultados resultados)
        {
            return new BilheteService().AtualizarResultados(resultados);
        }

        [HttpPost]
        [Route("reabrir-resultados")]
        [Authorize(Roles = "Administrador")]
        public string ReabrirResultados([FromBody] SorteioTab sorteio)
        {
            return new BilheteService().ReabrirResultados(sorteio.id);
        }

        [HttpPost]
        [Route("notificacao-bilhete")]
        [AllowAnonymous]
        public async Task<string> Notificacao([FromBody] NotificacaoMP notificacao)
        {
            return await new BilheteService().NotificacaoBilheteAsync(notificacao);
        }
    }
}