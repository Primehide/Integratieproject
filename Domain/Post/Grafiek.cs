using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Post
{
    public class Grafiek
    {
        public int GrafiekId { get; set; }
        //public double xas { get; set; }
        //public double yas { get; set; }
        //public double beginWaarde { get; set; }
        //public double limietwaarde { get; set; }
        public Domain.Enum.GrafiekType Type { get; set; }
        //public List<Post> Posts { get; set; }
        //public List<Entiteit.Entiteit> Entiteiten { get; set; }
        public List<GrafiekWaarde> Waardes { get; set; }
        public string Naam { get; set; }
    }
}
