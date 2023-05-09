using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using API.Models;
using API.Services.Financeiro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("v1/financeiro")]
    [ApiController]
    public class FinanceiroController : ControllerBase
    {
        [HttpPost]
        [Route("financeiro")]
        [Authorize(Roles = "Administrador, Gerente")]
        public List<FinanceiroGerenteTab> GetFinanceiro([FromBody] int sorteio)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            string id = User.Claims.First(c => c.Type == "Id").Value;

            return new FinanceiroService().Financeiro(id, tipo.Value, sorteio.ToString());
        }

        [HttpPost]
        [Route("caixa")]
        [Authorize(Roles = "Administrador, Vendedor")]
        public FinanceiroTab Caixa([FromBody] int sorteio)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            string id = User.Claims.First(c => c.Type == "Id").Value;

            return new FinanceiroService().Caixa(id, tipo.Value, sorteio.ToString());
        }
    }
}