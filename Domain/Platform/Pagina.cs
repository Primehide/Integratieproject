using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Platform
{
    public class Pagina
    {
        public int PaginaId { get; set; }
        public string Afbeelding { get; set; }
        public Domain.Entiteit.Entiteit Entiteit { get; set; }
    }
}
