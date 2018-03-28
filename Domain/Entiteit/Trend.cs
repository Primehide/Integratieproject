using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enum;

namespace Domain.Entiteit
{
    public class Trend
    {
        public int TrendId { get; set; }
        public TrendType Type { get; set; }
        public Voorwaarde Voorwaarde { get; set; }
    }
}
