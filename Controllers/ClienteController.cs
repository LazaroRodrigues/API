using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Models;
using API.Services.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("v1/cliente")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        [HttpPost]
        [Route("clientes")]
        [Authorize(Roles = "Administrador, Vendedor, Apostador")]
        public List<ClienteTab> Clientes([FromBody] UsuarioTab vendedor)
        {
            return new ClienteService().Clientes(vendedor.id);
        }

        [HttpPost]
        [Route("clientes-bolao")]
        [Authorize(Roles = "Vendedor, Apostador")]
        public List<ClienteTab> ClientesBolao([FromBody] int bolao)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            IEnumerable<Claim> claim = identity.Claims;

            var tipo = claim
                .Where(x => x.Type == ClaimTypes.Role)
                .FirstOrDefault();

            string id = User.Claims.First(c => c.Type == "Id").Value;

            return new ClienteService().ClientesBolao(id, bolao.ToString());
        }

        [HttpPut]
        [Route("criar-cliente")]
        [Authorize(Roles = "Administrador, Vendedor, Apostador")]
        public string CriarCliente([FromBody] ClienteTab cliente)
        {
            return new ClienteService().CriarCliente(cliente);
        }
    }
}