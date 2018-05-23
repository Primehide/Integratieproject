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
        public Domain.Enum.GrafiekType Type { get; set; }
        public List<Entiteit.Entiteit> Entiteiten { get; set; }
        public List<GrafiekWaarde> Waardes { get; set; }
        public string Naam { get; set; }
        public Domain.Enum.GrafiekSoort GrafiekSoort { get; set; }
        public Domain.Enum.SoortGegevens soortGegevens { get; set; }
        public List<CijferOpties> CijferOpties { get; set; }
    }
}
