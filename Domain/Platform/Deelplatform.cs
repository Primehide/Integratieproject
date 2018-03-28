using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Platform
{
    public class Deelplatform
    {
        public int DeelplatformId { get; set; }
        public string Naam { get; set; }
        public List<Domain.Entiteit.Entiteit> Entiteiten { get; set; }
        public List<Pagina> Paginas { get; set; }
    }
}
