using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class FinanceiroVendedorTab
    {
        public string nome { get; set; }
        public string id_gerente { get; set; }
        public decimal comissoes_vendedor { get; set; }
        public decimal apostas { get; set; }
        public decimal total { get; set; }
    }
}