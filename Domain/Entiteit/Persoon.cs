using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Persoon : Entiteit
    {
        public List<Organisatie> Organisaties { get; set; }
    }
}
