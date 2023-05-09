namespace API.Models
{
    public class PalpitesTab
    {
        public int id { get; set; }
        public string numero { get; set; }
        public int id_bilhete { get; set; }
        public int id_sorteio { get; set; }
        public string status { get; set; }
    }
}