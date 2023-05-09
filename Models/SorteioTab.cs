using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class SorteioTab
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string descricao { get; set; }
        public string status { get; set; }
        public decimal valor { get; set; }
        public string imagem { get; set; }
        public int quantidade { get; set; }
        public string data_sorteio { get; set; }
        public string solicitado_por { get; set; }
        public string resultado { get; set; }
    }
}