using System;

namespace API.Models
{
    public class RetornoMP
    {
        public DateTime date_created { get; set; }
        public string status { get; set; }
        public string status_detail { get; set; }
        public string description { get; set; }
        public string external_reference { get; set; }
        public bool live_mode { get; set; }
    }
}