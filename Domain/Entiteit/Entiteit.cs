using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entiteit
{
    public class Entiteit
    {
        public int EntiteitId { get; set; }
        public string naam { get; set; }
        public List<Trend> Trends { get; set; }
    }
}
