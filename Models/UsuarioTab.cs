using System;

namespace API.Models
{
    public class UsuarioTab
    {
        public string id { get; set; }
        public string usuario { get; set; }
        public string nome { get; set; }
        public string senha { get; set; }
        public string telefone { get; set; }
        public string cidade { get; set; }
        public string tipo { get; set; }
        public string status { get; set; }
        public DateTime data_criacao { get; set; }
        public string criado_por { get; set; }
        public string gerente { get; set; }
        public string gerenteNome { get; set; }
        public int comissao { get; set; }
    }
}