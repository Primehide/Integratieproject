using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Entiteit
    {
        [Key]
        public int EntiteitId { get; set; }
        public string Naam { get; set; }
        public List<Trend> Trends { get; set; }
        public List<Domain.Post.Post> Posts { get; set; }
    }
}
