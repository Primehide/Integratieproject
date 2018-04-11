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
              
                };
                ListBoxItems.Add(Organisation);
            }

            PersoonVM StartCreation = new PersoonVM()
            {
                OrganisationChecks = new SelectedOrganisationVM
                {
                    Organisations = ListBoxItems
                }
            };

            return View(StartCreation);
        }

        [HttpPost]
        public ActionResult AddPerson(PersoonVM pvm, IEnumerable<string> SelectedOrganisations)
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
            foreach (string oId in SelectedOrganisations)
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
        public ActionResult UpdatePerson(int EntityId)
        {

        List<SelectListItem> ListBoxItems = new List<SelectListItem>();

        List<Organisatie> AllOrganisations = eM.GetAllOrganisaties();
            foreach (Organisatie o in AllOrganisations)
            {
                    SelectListItem Organisation = new SelectListItem()
                    {
                        Text = o.Naam,
                        Value = o.EntiteitId.ToString(),

                    };
                    ListBoxItems.Add(Organisation);
            }

            UpdatePersonVM UPVM = new UpdatePersonVM
            {
                RequestedPerson = eM.GetPerson(EntityId),
                OrganisationChecks = new SelectedOrganisationVM
                {
                    Organisations = ListBoxItems
                }
            };


            return View(UPVM);
        }

                [HttpPost]
                public ActionResult UpdatePerson(UpdatePersonVM EditedPerson, IEnumerable<string> SelectedOrganisations)
                {
                        if (SelectedOrganisations != null)
                        {
                            List<Organisatie> NewlyAppointedOrganisations = new List<Organisatie>();
                            foreach (string oId in SelectedOrganisations)
                            {
                                NewlyAppointedOrganisations.Add(eM.GetOrganisatie(Int32.Parse(oId)));
                            }

                            EditedPerson.RequestedPerson.Organisations = NewlyAppointedOrganisations;
                        }
                    eM.ChangePerson(EditedPerson.RequestedPerson);
                    return View("DisplayPerson", EditedPerson.RequestedPerson);
                }
                #endregion
        // This region will handle the deletion of a certain person
        #region
        public ActionResult DeletePerson(int EntityId)
        {
            eM.RemovePerson(EntityId);
            return Index();
        }
        #endregion

        // This region will add a newly created Organisatie object to the database and persist
        #region
        public ActionResult AddOrganisation() 
        {
            List<SelectListItem> ListBoxItems = new List<SelectListItem>();

            List<Persoon> AllPeople = eM.GetAllPeople();
            foreach (Persoon p in AllPeople)
            {
                SelectListItem Person = new SelectListItem()
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EntiteitId.ToString(),

                };
                ListBoxItems.Add(Person);
            }

            OrganisatieVM StartCreation = new OrganisatieVM()
            {
                PeopleChecks = new SelectedPeopleVM
                {
                    People = ListBoxItems
                }
            };

            return View(StartCreation);
        }

        [HttpPost]
        public ActionResult AddOrganisation(OrganisatieVM newOrganisation, IEnumerable<string> SelectedPeople)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Organisatie AddedOrganisation = new Organisatie
            {
                Naam = newOrganisation.Name,
                Leden = new List<Persoon>(),
                Gemeente = newOrganisation.Town
            };

            foreach (string pId in SelectedPeople)
            {
                AddedOrganisation.Leden.Add(eM.GetPerson(Int32.Parse(pId)));
            }
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
            List<SelectListItem> ListBoxItems = new List<SelectListItem>();

            List<Persoon> AllPeople = eM.GetAllPeople();
            foreach (Persoon p in AllPeople)
            {
                SelectListItem Person = new SelectListItem()
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EntiteitId.ToString(),

                };
                ListBoxItems.Add(Person);
            }

            UpdateOrganisatieVM UOVM = new UpdateOrganisatieVM
            {
                RequestedOrganisatie = eM.GetOrganisatie(EntityId),
                PeopleChecks = new SelectedPeopleVM
                {
                    People = ListBoxItems
                }
            };

            return View(UOVM);
        }

        [HttpPost]
        public ActionResult UpdateOrganisation(UpdateOrganisatieVM editedOrganisation,IEnumerable<string> SelectedPeople)
        {
            if (SelectedPeople != null)
            {
                List<Persoon> NewlyAppointedPeople = new List<Persoon>();
                foreach (string pId in SelectedPeople)
                {
                    NewlyAppointedPeople.Add(eM.GetPerson(Int32.Parse(pId)));
                }

                    editedOrganisation.RequestedOrganisatie.Leden = NewlyAppointedPeople;
            }
            eM.ChangeOrganisatie(editedOrganisation.RequestedOrganisatie);
            return View("DisplayOrganisation", editedOrganisation.RequestedOrganisatie);
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