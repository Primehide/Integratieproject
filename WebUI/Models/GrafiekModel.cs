using System.Collections.Generic;

namespace WebUI.Models
{
    public class GrafiekModel
    {
        public string IdentityId { get; set; }
        public Domain.Enum.GrafiekType TypeGrafiek { get; set; }
        public bool AantalPosts { get; set; }
        public bool AantalRetweets { get; set; }
        public int Entiteit1 { get; set; }

        public string Naam { get; set; }
        public List<int> EntiteitIds { get; set; }
        public List<string> CijferOpties { get; set; }
        public string VergelijkOptie { get; set; }
        public Domain.Enum.GrafiekSoort GrafiekSoort { get; set; }
    }
}