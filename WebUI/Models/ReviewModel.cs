using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class ReviewModel
    {
        public List<Thema> Themas { get; set; }
        public List<Persoon> Personen { get; set; }
        public List<Organisatie> Organisaties { get; set; }
    }
}