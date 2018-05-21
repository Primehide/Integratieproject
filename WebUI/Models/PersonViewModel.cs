using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class PersonViewModel
    {
        public Persoon Persoon { get; set; }
        public int AantalPosts { get; set; }
        public int AantalPositieve { get; set; }
        public int AantalNegatieve { get; set; }
    }
}