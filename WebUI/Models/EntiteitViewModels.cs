using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Models
{
    public class PersoonVM
    {
        public string Fn { get; set; }
        public string Ln { get; set; }
        public IEnumerable<string> SelectedOrganisations { get; set; }
        public IEnumerable<SelectListItem> Organisations { get; set; }
    }

    public class OrganisatieVM
    {
        public string Name { get; set; }
        public string Town { get; set; }
        public IEnumerable<string> SelectedPeople { get; set; }
        public IEnumerable<SelectListItem> People { get; set; }
    }

    public class OverviewVM
    {
        public IEnumerable<Persoon> People { get; set; }
        public IEnumerable<Organisatie> Organisations { get; set; }
    }
}