using Domain.Entiteit;
using Domain.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class ChangePlatformViewModel
    {
        public Deelplatform requestedDeelplatform { get; set; }
        public List<Persoon> personen { get; set; }
        public List<Organisatie> organisaties { get; set; }
        public List<Thema> themas { get; set; }

    }
}