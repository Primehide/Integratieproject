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
using System.Net;
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
                People = eM.GetAllPeople((int)System.Web.HttpContext.Current.Session["PlatformID"]),
                Organisations = eM.GetAllOrganisaties((int)System.Web.HttpContext.Current.Session["PlatformID"])
            };
            return View(overview);
        }
        // Index Page for all Entities.
        [Authorize(Roles = "SuperAdmin, Admin")]

        public virtual ActionResult Test()
        {
            //List<Entiteit> AllEntities = new List<Entiteit>();
            //AllEntities.AddRange(eM.GetAllPeople());
            //AllEntities.AddRange(eM.GetAllOrganisaties());

            return View();
        }

        public void BerekenVasteGrafiekenAlleEntiteiten()
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            entiteitManager.BerekenVasteGrafiekenAlleEntiteiten();
        }

        public ActionResult AddEntiteit(Entiteit entiteit)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            entiteitManager.addEntiteit(entiteit);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AddPersoon(Persoon p, string organisatie, HttpPostedFileBase uploadFile)
        {
            fillOrganisaties();
           
            EntiteitManager entiteitManager = new EntiteitManager();
            p.Organisations = new List<Organisatie>();
            int organisationId = NaamType.Keys.Where(x => x.Naam == organisatie).FirstOrDefault().EntiteitId;
            p.Organisations.Add(entiteitManager.GetOrganisatie(organisationId));
            entiteitManager.AddPerson(p,uploadFile);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AddOrganisatie(Organisatie o, HttpPostedFileBase uploadFile)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            o.Leden = new List<Persoon>();
            entiteitManager.AddOrganisatie(o, uploadFile);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        public ActionResult PersoonPagina(int id)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            return View(entiteitManager.GetPerson(id));
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
                    List<Domain.Entiteit.Persoon> JsonEntiteiten = JsonConvert.DeserializeObject<List<Domain.Entiteit.Persoon>>(str);
                    foreach (var p in JsonEntiteiten)
                    {
                        p.PlatformId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
                    }
                    entiteitManager.ConvertJsonToEntiteit(JsonEntiteiten);
                }
            }
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AddThema(Thema t, string woorden, HttpPostedFileBase uploadFile)
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
            entiteitManager.AddThema(t, sleutelWoorden, uploadFile);
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult AddPerson(int platformId)
        {
            List<SelectListItem> ListBoxItems = new List<SelectListItem>();

            List<Organisatie> AllOrganisations = eM.GetAllOrganisaties((int)System.Web.HttpContext.Current.Session["PlatformID"]);
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
        [Authorize(Roles = "SuperAdmin, Admin")]
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
                PlatformId = pvm.platId,
                Naam = pvm.Fn + " " + pvm.Ln
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

            return RedirectToAction("AdminBeheerEntiteiten", "Account");

        }
        #endregion
  

        // This region is for displaying a certain person, given that a certain entityId is given.
        #region
        public virtual ActionResult DisplayPerson(int EntityId)
        {
            PostManager mgr = new PostManager();
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
                AantalNegatieve = polariteitNegatiefCount,
                AantalMentions = mgr.getAantalMentions(ToDisplay),
                TopWords = mgr.getTopPersonWords(ToDisplay)
               

            };





            return View(personViewModel);
        }
        #endregion

        // This region will handle the updating of a certain person. After the update you will be redirected to the Display page of the updated person;
        #region
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdatePerson(int EntityId)
        {

            List<SelectListItem> ListBoxItems = new List<SelectListItem>();

            List<Organisatie> AllOrganisations = eM.GetAllOrganisaties((int)System.Web.HttpContext.Current.Session["PlatformID"]);
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdatePerson(UpdatePersonVM EditedPerson, IEnumerable<string> SelectedOrganisations, HttpPostedFileBase uploadFile)
        {


            if (SelectedOrganisations != null)
            {

                eM.ChangePerson(EditedPerson.RequestedPerson, SelectedOrganisations, uploadFile);


            }
            else
            {
                eM.ChangePerson(EditedPerson.RequestedPerson, uploadFile);
            }

            return Redirect("~/Account/AdminBeheerEntiteiten");
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult AddOrganisation(int platformId)
        {
            List<SelectListItem> ListBoxItems = new List<SelectListItem>();

            List<Persoon> AllPeople = eM.GetAllPeople((int)System.Web.HttpContext.Current.Session["PlatformID"]);
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
        [Authorize(Roles = "SuperAdmin, Admin")]
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

            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }
        #endregion

        // This region is for displaying a certain Organisatie object, given that a certain entityId is given.
        #region
        public virtual ActionResult DisplayOrganisation(int EntityId)
        {

            Organisatie ToDisplay = eM.GetOrganisatie(EntityId);
            if ((int)System.Web.HttpContext.Current.Session["PlatformID"] == ToDisplay.EntiteitId)
            {
                return View(ToDisplay);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }
        #endregion

        // This region will handle the updating of a certain Organisation. After the update you will be redirected to the Display page of the updated organisation;
        #region
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdateOrganisation(int EntityId)
        {
            List<SelectListItem> ListBoxItems = new List<SelectListItem>();

            List<Persoon> AllPeople = eM.GetAllPeople((int)System.Web.HttpContext.Current.Session["PlatformID"]);
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdateOrganisation(UpdateOrganisatieVM vm, IEnumerable<string> SelectedPeople, HttpPostedFileBase uploadFile)
        {

            if (SelectedPeople != null)
            {
                eM.ChangeOrganisatie(vm.RequestedOrganisatie, SelectedPeople, uploadFile);

            }
            else
            {
                eM.ChangeOrganisatie(vm.RequestedOrganisatie, uploadFile);
            }
        
            return Redirect("~/Account/AdminBeheerEntiteiten");
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
        [Authorize(Roles = "SuperAdmin, Admin")]
        public void CreateTestData()
        {
            BL.EntiteitManager entiteitManager = new BL.EntiteitManager();
            entiteitManager.CreateTestData();
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult IndexThema()
        {
            IEnumerable<Thema> themas = eM.GetThemas((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            return View(themas);
        }

        // GET: Thema/Create
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult CreateThema(int platid)
        {

            return View(new Thema { PlatformId = platid });
        }
        // POST: Thema/Create
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult CreateThema(Thema thema, List<Sleutelwoord> sleutelwoorden, HttpPostedFileBase uploadFile)
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
                eM.AddThema(thema, mijnList, uploadFile);
                return RedirectToAction("IndexThema");
            }
            return View();
        }


        // GET: Thema/Edit/
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult EditThema(int id)
        {
            return View(eM.GetThema(id));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult EditThema(Thema thema, List<Sleutelwoord> sleutelwoorden, HttpPostedFileBase uploadFile)
        {

            //  TempData["themaID"] = thema.EntiteitId;
            var mijnThema = eM.GetThema(thema.EntiteitId);
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
            eM.UpdateThema(thema, uploadFile);
            return Redirect("~/Account/AdminBeheerEntiteiten");
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThema(int id)
        {
            
            TempData["themaID"] = eM.GetThema(id);
            return View(eM.GetThema(id));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThema(int id,  FormCollection collection)
        {
            eM.DeleteThema(id);
            return RedirectToAction("IndexThema");                                        
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThemaSleutelwoord(int id, int themaID)
        {
            TempData["themaID"] = themaID ;
            eM.DeleteSleutelwoord(id);
            Thema thema = eM.GetThema(themaID);
            return View("~/Views/Entiteit/EditThema.cshtml", thema);
        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThemaSleutelwoord(int id, int themaID, FormCollection collection)
        {

            return RedirectToAction("IndexThema");
            // return View();
        }

        public ActionResult ZoekEntiteit(string naam)
        {
            List<Entiteit> entiteiten = eM.GetEntiteiten(naam);
            TempData["MyList"] = entiteiten.ToList();
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

        Dictionary<Entiteit, string> NaamType = new Dictionary<Entiteit, string>();
        private void fillOrganisaties()
        {

            ArrayList organisaties = new ArrayList();

            List<Entiteit> entiteits = new List<Entiteit>();

            EntiteitManager mgr = new EntiteitManager();
            entiteits = mgr.getAlleEntiteiten();
            if (NaamType.Count == 0)
            {
                foreach (Entiteit entiteit in entiteits)
                {

                    if (entiteit is Organisatie)
                    {
                        NaamType.Add(entiteit, "Organisatie");
                    }



                }
            }
            NaamType.ToList().ForEach(x => organisaties.Add(x.Key.Naam));
            ViewBag.Organisaties = organisaties;
        }
   
    }
}