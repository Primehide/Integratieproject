using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class GrafiekViewModel
    {
        public List<Domain.Entiteit.Persoon> Personen { get; set; }
        public List<Domain.Entiteit.Organisatie> Organisaties { get; set; }
        public List<Domain.Entiteit.Thema> Themas { get; set; }
    }
}