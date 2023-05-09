using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class BilheteTab
    {
        public int id { get; set; }
        public decimal valor { get; set; }
        public string codigo { get; set; }
        public int id_cliente { get; set; }
        public string nome_cliente { get; set; }
        public string cidade_cliente { get; set; }
        public string telefone_cliente { get; set; }
        public string id_vendedor { get; set; }
        public string id_gerente { get; set; }
        public string nome_vendedor { get; set; }
        public string premio { get; set; }
        public string data_sorteio { get; set; }
        public string status { get; set; }
        public string origem { get; set; }
        public int id_sorteio { get; set; }
        public DateTime data { get; set; }
        public string cancelado_por { get; set; }
        public DateTime data_cancelamento { get; set; }
        public List<PalpitesTab> palpites { get; set; }
        public bool erro { get; set; }
        public string erroMensagem { get; set; }
        public string imagemPix { get; set; }
        public string textoPix { get; set; }
        public string sitePix { get; set; }
    }
}