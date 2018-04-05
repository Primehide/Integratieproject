using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Organisatie : Entiteit
    {
        public int OrganisatieId { get; set; }
        public string Naam { get; set; }
        public int AantalLeden { get; set; }
        public string Gemeente { get; set; }
        public List<Persoon> Leden { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}
