using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models
{
    public class PersoonVM
    {
        public string Fn { get; set; }
        public string Ln { get; set; }
        public Organisatie[] Organisations { get; set; }
    }

    public class OrganisatieVM
    {
        public string Name { get; set; }
        public string Town { get; set; }
        public Persoon[] People { get; set; }
    }
}