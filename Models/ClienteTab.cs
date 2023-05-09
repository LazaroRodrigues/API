using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class ClienteTab
    {
        public int id { get; set; }
        public string nome { get; set; }
        public string telefone { get; set; }
        public string cidade { get; set; }
        public string id_vendedor { get; set; }
    }
}