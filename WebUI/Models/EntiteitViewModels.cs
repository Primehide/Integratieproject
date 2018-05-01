using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Models
{
    //Region regarding classes used in Person pages
    #region
    public class PersoonVM
    {
        public string Fn { get; set; }
        public string Ln { get; set; }
        public byte[] Image { get; set; }
        public int platId { get; set; }
        public SelectedOrganisationVM OrganisationChecks { get; set; }
    }

    public class UpdatePersonVM
    {
        public Persoon RequestedPerson { get; set; }
        public SelectedOrganisationVM OrganisationChecks { get; set; }
    }

    public class SelectedOrganisationVM
    {
        public IEnumerable<SelectListItem> Organisations { get; set; }
    }
    #endregion

    //Region regarding classes used in Organisation pages
    #region
    public class OrganisatieVM
    {
        public string Name { get; set; }
        public string Town { get; set; }
        public byte[] Image { get; set; }
        public int platId { get; set; }
        public SelectedPeopleVM PeopleChecks { get; set; }
    }

    public class UpdateOrganisatieVM
    {
        public Organisatie RequestedOrganisatie { get; set; }
        public SelectedPeopleVM PeopleChecks { get; set; }
    }

    public class SelectedPeopleVM
    {
        public IEnumerable<SelectListItem> People { get; set; }
    }
    #endregion

    //Region regarding the overview page (Testing)
    #region
    public class OverviewVM
    {
        public IEnumerable<Persoon> People { get; set; }
        public IEnumerable<Organisatie> Organisations { get; set; }
    }

    #endregion
}