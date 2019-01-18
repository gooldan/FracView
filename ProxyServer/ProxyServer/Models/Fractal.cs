using System.ComponentModel.DataAnnotations;

namespace ProxyServer.Models
{
    public class Fractal
    {
        [Key]
        public string id { get; set; }
        public string url { get; set; }
        public string alpha { get; set; }
        public string beta { get; set; }
        public string gamma { get; set; }
        public int x { get; set; }
        public int y { get; set; }        
    }
}
