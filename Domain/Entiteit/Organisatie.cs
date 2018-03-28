using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Organisatie : Entiteit
    {
        public string gemeente { get; set; }
        public List<Persoon> Leden { get; set; }
    }
}
