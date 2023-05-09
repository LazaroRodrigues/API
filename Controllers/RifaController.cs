using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Services.Sorteio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("v1/sorteios")]
    [ApiController]
    public class RifaController : ControllerBase
    {
        [HttpGet]
        [Route("sorteios")]
        [Authorize(Roles = "Administrador, Vendedor, Cliente, Gerente, Apostador")]
        public List<SorteioTab> GetSorteios()
        {
            return new SorteioService().ListaSorteios();
        }

        [HttpPut]
        [Route("criar-sorteio")]
        [Authorize(Roles = "Administrador")]
        public string CriarSorteio([FromBody] SorteioTab sorteio)
        {
            return new SorteioService().CriarSorteio(sorteio);
        }

        [HttpPost]
        [Route("editar-sorteio")]
        [Authorize(Roles = "Administrador")]
        public string EditarSorteio([FromBody] SorteioTab sorteio)
        {
            return new SorteioService().EditarSorteio(sorteio);
        }

        [HttpPost]
        [Route("excluir-sorteio")]
        [Authorize(Roles = "Administrador")]
        public string ExcluirSorteio([FromBody] SorteioTab sorteio)
        {
            return new SorteioService().ExcluirSorteio(sorteio);
        }

        [HttpPost]
        [Route("atualizar-status")]
        [Authorize(Roles = "Administrador")]
        public string AtualizarStatus([FromBody] SorteioTab sorteio)
        {
            return new SorteioService().AtualizarStatus(sorteio);
        }

        [HttpPost]
        [Route("lancar-resultado")]
        [Authorize(Roles = "Administrador")]
        public string LancarResultado([FromBody] SorteioTab sorteio)
        {
            return new SorteioService().LancarResultado(sorteio);
        }
    }
}