using Domain.Post;
using System.Collections.Generic;

namespace WebUI.Models
{
    public class GrafiekViewModel
    {
        public Grafiek Grafiek { get; set; }
        public List<Domain.Entiteit.Persoon> Personen { get; set; }
        public List<Domain.Entiteit.Organisatie> Organisaties { get; set; }
        public List<Domain.Entiteit.Thema> Themas { get; set; }
    }
}