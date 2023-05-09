namespace API.Models
{
    public class FinanceiroTab
    {
        public string nome { get; set; }
        public decimal comissoes_vendedor { get; set; }
        public decimal comissoes_gerente { get; set; }
        public decimal apostas { get; set; }
        public decimal total { get; set; }
    }
}