using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class GrafiekModel
    {
        public string IdentityId { get; set; }
        public Domain.Enum.GrafiekType TypeGrafiek { get; set; }
        public bool aantalPosts { get; set; }
        public bool aantalRetweets { get; set; }
        public int Entiteit1 { get; set; }

        public List<int> EntiteitIds { get; set; }
        public List<string> CijferOpties { get; set; }
        public string VergelijkOptie { get; set; }
    }
}