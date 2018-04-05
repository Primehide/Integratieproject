using BL;
using Domain.Entiteit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class EntiteitController : Controller
    {
        private EntiteitManager eM = new EntiteitManager();

        // Index Page for all Entities.
        public ActionResult Index()
        {
            //List<Entiteit> AllEntities = new List<Entiteit>();
            //AllEntities.AddRange(eM.GetAllPeople());
            //AllEntities.AddRange(eM.GetAllOrganisaties());
            OverviewVM overview = new OverviewVM
            {
                People = eM.GetAllPeople(),
                Organisations = eM.GetAllOrganisaties()
            };
            return View(overview);
        }

        // Index Page for all Entities.
        public ActionResult Test()
        {
            //List<Entiteit> AllEntities = new List<Entiteit>();
            //AllEntities.AddRange(eM.GetAllPeople());
            //AllEntities.AddRange(eM.GetAllOrganisaties());

            return View();
        }

        // This region is for adding a person to the database and persisting.
        #region
        public ActionResult AddPerson()
        {
            List<SelectListItem> ListBoxItems = new List<SelectListItem>();

            List<Organisatie> AllOrganisations = eM.GetAllOrganisaties();
            foreach (Organisatie o in AllOrganisations)
            {
                SelectListItem Organisation = new SelectListItem()
                {
                    Text = o.Naam,
                    Value = o.EntiteitId.ToString(),
                    Selected = o.IsSelected
                };
                ListBoxItems.Add(Organisation);
            }

            PersoonVM StartCreation = new PersoonVM()
            {
                Organisations = ListBoxItems
            };

            return View(StartCreation);
        }

        [HttpPost]
        public ActionResult AddPerson(PersoonVM pvm)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            Persoon AddedPerson = new Persoon
            {
                LastName = pvm.Ln,
                Organisations = new List<Organisatie>(),
                FirstName = pvm.Fn
            };
            foreach (string oId in pvm.SelectedOrganisations)
            {
                AddedPerson.Organisations.Add(eM.GetOrganisatie(Int32.Parse(oId)));
            }
            eM.AddPerson(AddedPerson);
            return View("DisplayPerson", AddedPerson);
        }
        #endregion

        // This region is for displaying a certain person, given that a certain entityId is given.
        #region
        public ActionResult DisplayPerson(int EntityId)
        {
            Persoon ToDisplay = eM.GetPerson(EntityId);
            return View(ToDisplay);
        }
        #endregion
        // This region will handle the updating of a certain person. After the update you will be redirected to the Display page of the updated person;
        #region
                public ActionResult UpdatePerson(int PersonId)
                {
                    return View(eM.GetPerson(PersonId));
                }

                [HttpPost]
                public ActionResult UpdatePerson(Persoon EditedPerson)
                {
                    eM.ChangePerson(EditedPerson);
                    return View("DisplayPerson", EditedPerson);
                }
                #endregion
        // This region will handle the deletion of a certain person
        #region
        public ActionResult DeletePerson(int PersonId)
        {
            eM.RemovePerson(PersonId);
            return Index();
        }
        #endregion

        // This region will add a newly created Organisatie object to the database and persist
        #region
        public ActionResult AddOrganisation() 
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddOrganisation(OrganisatieVM newOrganisation)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Organisatie AddedOrganisation = new Organisatie
            {
                Naam = newOrganisation.Name,
                Gemeente = newOrganisation.Town
            };

            eM.AddOrganisatie(AddedOrganisation);
            return View("DisplayOrganisation", AddedOrganisation);
        }
        #endregion


        // This region is for displaying a certain Organisatie object, given that a certain entityId is given.
        #region
        public ActionResult DisplayOrganisation(int EntityId)
        {
            Organisatie ToDisplay = eM.GetOrganisatie(EntityId);
            return View(ToDisplay);
        }
        #endregion
        // This region will handle the updating of a certain organisation. After the update you will be redirected to the Display page of the updated organisation;
        #region
        public ActionResult UpdateOrganisation(int EntityId)
        {
            return View(eM.GetOrganisatie(EntityId));
        }

        [HttpPost]
        public ActionResult UpdateOrganisation(Organisatie editedOrganisation)
        {
            eM.ChangeOrganisatie(editedOrganisation);
            return View("DisplayOrganisation", editedOrganisation);
        }
        #endregion
        // This region will handle the deletion of a certain person
        #region
        public ActionResult DeleteOrganisation(int EntityId)
        {
            eM.RemoveOrganisatie(EntityId);
            OverviewVM overview = new OverviewVM
            {
                People = eM.GetAllPeople(),
                Organisations = eM.GetAllOrganisaties()
            };
            return View("Index",overview);
        }
        #endregion
    }
}