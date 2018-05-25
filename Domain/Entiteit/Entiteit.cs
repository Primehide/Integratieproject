using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entiteit
{
    public class Entiteit
    {
        [Key]
        public int EntiteitId { get; set; }
        public int PlatformId { get; set; }
        public string Naam { get; set; }
        public List<Trend> Trends { get; set; }
        public List<Domain.Post.Post> Posts { get; set; }
        public List<Domain.Post.Grafiek> Grafieken { get; set; }
        public List<Domain.Post.Word> Words { get; set; }
        public byte[] Image { get; set; }
    }
}
