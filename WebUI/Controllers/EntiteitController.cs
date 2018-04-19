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

            if (SelectedOrganisations != null)
            {
                foreach (string oId in SelectedOrganisations)
                {
                    //Organisatie WorkingOrganisation = eM.GetOrganisatie(Int32.Parse(oId));
                    //WorkingOrganisation.Leden.Add(AddedPerson);
                    //WorkingOrganisation.AantalLeden = WorkingOrganisation.Leden.Count();
                    AddedPerson.Organisations.Add(eM.GetOrganisatie(Int32.Parse(oId)));
                }
            }

            HttpPostedFileBase file = null;

            foreach (HttpPostedFileBase hpfb in Request.Files)
            {
                file = hpfb;
            }

            eM.AddPerson(AddedPerson,file);

            return RedirectToAction("Index");

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

                eM.ChangePerson(EditedPerson.RequestedPerson, SelectedOrganisations);

                        
            } else
            {
                eM.ChangePerson(EditedPerson.RequestedPerson);
            }

         return RedirectToAction("Index");
        }
         #endregion

        // This region will handle the deletion of a certain person
        #region
        public ActionResult DeletePerson(int EntityId)
        {
            eM.RemovePerson(EntityId);
            return RedirectToAction("Index");
        }
        #endregion

        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = null;
            Entiteit e = eM.GetPerson(id);
            if (e == null)
            {
                e = eM.GetOrganisatie(id);
            }

            if (e.GetType() == typeof(Persoon))
            {
                cover = eM.GetPersonImageFromDataBase(id);
            }
            else if (e.GetType() == typeof(Organisatie))
            {
                cover = eM.GetOrganisationImageFromDataBase(id);
            }

            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            { 
                return null;
            }
        }

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
            if (SelectedPeople != null)
            {
                foreach (string pId in SelectedPeople)
                {
                    AddedOrganisation.Leden.Add(eM.GetPerson(Int32.Parse(pId)));
                }
            }
            HttpPostedFileBase file = Request.Files["ImageData"];

            eM.AddOrganisatie(AddedOrganisation, file);

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

        // This region will handle the updating of a certain Organisation. After the update you will be redirected to the Display page of the updated organisation;
        // TODO : Application of UOW to prevent double creation of an Organisatie Object
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
                eM.ChangeOrganisatie(editedOrganisation.RequestedOrganisatie, SelectedPeople);

            } else
            {
                eM.ChangeOrganisatie(editedOrganisation.RequestedOrganisatie);
            }

            return RedirectToAction("Index");
        }
        #endregion

        // This region will handle the deletion of a certain Organisation
        #region
        public ActionResult DeleteOrganisation(int EntityId)
        {
            eM.RemoveOrganisatie(EntityId);

            return RedirectToAction("Index");
        }
        #endregion

        public void CreateTestData()
        {
            BL.EntiteitManager entiteitManager = new BL.EntiteitManager();
            entiteitManager.CreateTestData();
        }

    }
}