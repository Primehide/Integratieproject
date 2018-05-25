using Domain.Entiteit;
using Domain.Platform;
using System.Collections.Generic;

namespace WebUI.Models
{
    public class ChangePlatformViewModel
    {
        public Deelplatform RequestedDeelplatform { get; set; }
        public List<Persoon> Personen { get; set; }
        public List<Organisatie> Organisaties { get; set; }
        public List<Thema> Themas { get; set; }

    }
}