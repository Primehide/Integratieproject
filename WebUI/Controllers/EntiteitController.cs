using BL;
using Domain.Entiteit;
using Domain.Post;
using Domain.TextGain;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public partial class EntiteitController : Controller
    {
        private EntiteitManager eM = new EntiteitManager();

        // Index Page for all Entities.
        public virtual ActionResult Index()
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
        public virtual ActionResult Test()
        {
            //List<Entiteit> AllEntities = new List<Entiteit>();
            //AllEntities.AddRange(eM.GetAllPeople());
            //AllEntities.AddRange(eM.GetAllOrganisaties());

            return View();
        }

        public ActionResult AddEntiteit(Entiteit entiteit)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            entiteitManager.addEntiteit(entiteit);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        [HttpPost]
        public ActionResult AddPersoon(Persoon p, int organisationId)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            p.Organisations = new List<Organisatie>();
            p.Organisations.Add(entiteitManager.GetOrganisatie(organisationId));
            entiteitManager.AddPerson(p,null);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        public ActionResult AddOrganisatie(Organisatie o)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            o.Leden = new List<Persoon>();
            entiteitManager.AddOrganisatie(o, null);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        public ActionResult PersoonPagina()
        {
            return View();
        }

        public ActionResult Zoeken(string zoekwoord)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            List<Entiteit> entiteiten = entiteitManager.ZoekEntiteiten(zoekwoord);
            return View(entiteiten);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    string str = (new StreamReader(file.InputStream)).ReadToEnd();
                    List<Domain.TextGain.JsonEntiteit> JsonEntiteiten = JsonConvert.DeserializeObject<List<Domain.TextGain.JsonEntiteit>>(str);
                    entiteitManager.ConvertJsonToEntiteit(JsonEntiteiten);
                }
            }
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        [HttpPost]
        public ActionResult AddThema(Thema t, string woorden)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            string[] split = woorden.Split(',');
            List<Sleutelwoord> sleutelWoorden = new List<Sleutelwoord>();
            foreach (var woord in split)
            {
                Sleutelwoord sleutelwoord = new Sleutelwoord();
                sleutelwoord.woord = woord;
                sleutelWoorden.Add(sleutelwoord);
            }
            entiteitManager.AddThema(t, sleutelWoorden);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }
        // this region is for displaying a certain theme
        #region
        public virtual ActionResult DisplayTheme(int EntityId)
        {
            Thema ToDisplay = eM.GetThema(EntityId);
            return View(ToDisplay);
        }
        #endregion

        // This region is for adding a person to the database and persisting.
        #region
        public virtual ActionResult AddPerson(int platformId)
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
                platId = platformId,
                OrganisationChecks = new SelectedOrganisationVM
                {
                    Organisations = ListBoxItems
                }
            };

            return View(StartCreation);
        }

        [HttpPost]
        public virtual ActionResult AddPerson(PersoonVM pvm, IEnumerable<string> SelectedOrganisations)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }



            Persoon AddedPerson = new Persoon
            {
                LastName = pvm.Ln,
                Organisations = new List<Organisatie>(),
                FirstName = pvm.Fn,
                PlatformId = pvm.platId
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

            eM.AddPerson(AddedPerson, file);

            return RedirectToAction("Index");

        }
        #endregion

        // This region is for displaying a certain person, given that a certain entityId is given.
        #region
        public virtual ActionResult DisplayPerson(int EntityId)
        {
            Persoon ToDisplay = eM.GetPerson(EntityId);
            List<Post> posts = ToDisplay.Posts;
            List<Double> polariteitPositief = new List<double>();
           
            foreach (Post post in posts)
            {
                if (post.Sentiment.polariteit >= 0)
                {
                    double waarde = post.Sentiment.polariteit;
                    polariteitPositief.Add(waarde);
                } 

            }
            int polariteitNegatiefCount = posts.Count() - polariteitPositief.Count();
            int totaal = posts.Count();
            int polariteitPositiefCount = polariteitPositief.Count();
            PersonViewModel personViewModel = new PersonViewModel()
            {
                Persoon = ToDisplay,
                AantalPosts = totaal,
                AantalPositieve = polariteitPositiefCount,
                AantalNegatieve = polariteitNegatiefCount
            };
            return View(personViewModel);
        }
        #endregion

        // This region will handle the updating of a certain person. After the update you will be redirected to the Display page of the updated person;
        #region
        public virtual ActionResult UpdatePerson(int EntityId)
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
        public virtual ActionResult UpdatePerson(UpdatePersonVM EditedPerson, IEnumerable<string> SelectedOrganisations)
        {

            if (SelectedOrganisations != null)
            {

                eM.ChangePerson(EditedPerson.RequestedPerson, SelectedOrganisations);


            }
            else
            {
                eM.ChangePerson(EditedPerson.RequestedPerson);
            }

            return RedirectToAction("Index");
        }
        #endregion

        // This region will handle the deletion of a certain person
        #region
        public virtual ActionResult DeletePerson(int EntityId)
        {
            eM.RemovePerson(EntityId);
            return RedirectToAction("Index");
        }
        #endregion

        public virtual ActionResult RetrieveImage(int id)
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
        public virtual ActionResult AddOrganisation(int platformId)
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
                platId = platformId,
                PeopleChecks = new SelectedPeopleVM
                {
                    People = ListBoxItems
                }
            };

            return View(StartCreation);
        }

        [HttpPost]
        public virtual ActionResult AddOrganisation(OrganisatieVM newOrganisation, IEnumerable<string> SelectedPeople)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Organisatie AddedOrganisation = new Organisatie
            {
                Naam = newOrganisation.Name,
                Leden = new List<Persoon>(),
                PlatformId = newOrganisation.platId,
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
        public virtual ActionResult DisplayOrganisation(int EntityId)
        {
            Organisatie ToDisplay = eM.GetOrganisatie(EntityId);
            return View(ToDisplay);
        }
        #endregion

        // This region will handle the updating of a certain Organisation. After the update you will be redirected to the Display page of the updated organisation;
        // TODO : Application of UOW to prevent double creation of an Organisatie Object
        #region
        public virtual ActionResult UpdateOrganisation(int EntityId)
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
        public virtual ActionResult UpdateOrganisation(UpdateOrganisatieVM editedOrganisation, IEnumerable<string> SelectedPeople)
        {
            if (SelectedPeople != null)
            {
                eM.ChangeOrganisatie(editedOrganisation.RequestedOrganisatie, SelectedPeople);

            }
            else
            {
                eM.ChangeOrganisatie(editedOrganisation.RequestedOrganisatie);
            }

            return RedirectToAction("Index");
        }
        #endregion

        // This region will handle the deletion of a certain Organisation
        #region
        public virtual ActionResult DeleteOrganisation(int EntityId)
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

        public virtual ActionResult IndexThema()
        {
            IEnumerable<Thema> themas = eM.GetThemas();
            return View(themas);
        }

        // GET: Thema/Create
        public virtual ActionResult CreateThema(int platid)
        {

            return View(new Thema { PlatformId = platid });
        }
        // POST: Thema/Create
        [HttpPost]
        public virtual ActionResult CreateThema(Thema thema, List<Sleutelwoord> sleutelwoorden)
        {
            // sleutelwoorden.RemoveAll(item => item.woord == null);
            string woorden = sleutelwoorden[0].woord;
            string[] split = woorden.Split(',');
            List<Sleutelwoord> mijnList = new List<Sleutelwoord>();
            foreach (string woord in split)
            {
                Sleutelwoord sleutelwoord = new Sleutelwoord(woord);
                mijnList.Add(sleutelwoord);
            }
            if (ModelState.IsValid)
            {
                eM.AddThema(thema, mijnList);
                return RedirectToAction("IndexThema");
            }
            return View();
        }


        // GET: Thema/Edit/
        public virtual ActionResult EditThema(int id)
        {
            return View(eM.GetThema(id));
        }

        [HttpPost]
        public virtual ActionResult EditThema(Thema thema, int id, List<Sleutelwoord> sleutelwoorden)
        {
            thema.EntiteitId = id;

            var mijnThema = eM.GetThema(id);
            string woorden = sleutelwoorden[0].woord;
            if (woorden != null)
            {
                string[] split = woorden.Split(',');
                List<Sleutelwoord> mijnList = mijnThema.SleutenWoorden;
                foreach (string woord in split)
                {
                    Sleutelwoord sleutelwoord = new Sleutelwoord(woord);
                    mijnList.Add(sleutelwoord);
                }
                thema.SleutenWoorden = mijnList;
            }
            eM.UpdateThema(thema);
            return RedirectToAction("IndexThema");
        }


        public virtual ActionResult DeleteThema(int id)
        {
            return View(eM.GetThema(id));
        }

        [HttpPost]
        public virtual ActionResult DeleteThema(int id, FormCollection collection)
        {
            eM.DeleteThema(id);
            return RedirectToAction("IndexThema");                                        
        }

        public virtual ActionResult DeleteThemaSleutelwoord(int id)
        {
            return View(eM.GetSleutelwoord(id));
        }
        [HttpPost]
        public virtual ActionResult DeleteThemaSleutelwoord(int id, FormCollection collection)
        {
            eM.DeleteSleutelwoord(id);
            return RedirectToAction("IndexThema");
            // return View();
        }


        public ActionResult ZoekEntiteit(string naam)
        {
            List<Entiteit> entiteiten = eM.GetEntiteiten(naam);
            TempData["myList"] = entiteiten.ToList();
            return RedirectToAction("ShowEntiteiten");
        }

        public ActionResult ShowEntiteiten()
        {
            List<Persoon> deelplatformPersonen = new List<Persoon>();
            List<Organisatie> deelplatformOrganisaties = new List<Organisatie>();
            List<Thema> deelplatformThemas = new List<Thema>();

            var model = TempData["myList"] as List<Entiteit>;


            foreach (Entiteit e in model)
            {
                if (e is Persoon)
                {
                    deelplatformPersonen.Add((Persoon)e);
                }
                else
                if (e is Organisatie)
                {
                    deelplatformOrganisaties.Add((Organisatie)e);
                }
                else
                if (e is Thema)
                {
                    deelplatformThemas.Add((Thema)e);
                }
                
            }
            ZoekModel zoekModel = new ZoekModel()
            {
                Themas = deelplatformThemas,
                Personen = deelplatformPersonen,
                Organisaties = deelplatformOrganisaties
            };
            return View(zoekModel);
        }

    }
}