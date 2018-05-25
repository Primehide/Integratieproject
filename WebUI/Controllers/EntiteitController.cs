using BL;
using Domain.Entiteit;
using Domain.Post;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL.Managers;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class EntiteitController : Controller
    {
        // Index Page for all Entities.
        public virtual ActionResult Index()
        { 
            //OverviewVM overview = new OverviewVM
            var entiteitManager = new EntiteitManager();
            var platformId = (int) System.Web.HttpContext.Current.Session["PlatformID"];
            var overview = new OverviewVM
            {
                People = entiteitManager.GetAllPeople(platformId),
                Organisations = entiteitManager.GetAllOrganisaties(platformId)
            };
            return View(overview);
        }

        public ActionResult ShowEntities()
        {
            var entiteitManager = new EntiteitManager();
            return View(entiteitManager.GetEntiteitenVanDeelplatform((int)System.Web.HttpContext.Current.Session["PlatformID"]));
        }

        public void BerekenVasteGrafiekenAlleEntiteiten()
        {
            var entiteitManager = new EntiteitManager();
            entiteitManager.BerekenVasteGrafiekenAlleEntiteiten();
        }

        public ActionResult AddEntiteit(Entiteit entiteit)
        {
            var entiteitManager = new EntiteitManager();
            entiteitManager.AddEntiteit(entiteit);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        // rare methode?????? lege naamtype daar een pipeline op??
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AddPersoon(Persoon p, string organisatie, HttpPostedFileBase uploadFile)
        {
            FillOrganisaties();
            EntiteitManager entiteitManager = new EntiteitManager();
            p.Organisations = new List<Organisatie>();
            int organisationId = FillOrganisaties().Keys.Where(x => x.Naam == organisatie).FirstOrDefault().EntiteitId;
            p.Organisations.Add(entiteitManager.GetOrganisatie(organisationId));
            p.PlatformId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
            entiteitManager.AddPerson(p,uploadFile);

            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        //foreach lus naar manager
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AddOrganisatie(Organisatie o, HttpPostedFileBase uploadFile, IEnumerable<string> selectedPeople)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            if (selectedPeople != null)
            {
                o.Leden = new List<Persoon>();
                foreach (string pId in selectedPeople)
                {
                    o.Leden.Add(entiteitManager.GetPerson(int.Parse(pId)));
                }
            }
            o.PlatformId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
            entiteitManager.AddOrganisatie(o, uploadFile);

            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        public ActionResult PersoonPagina(int id)
        {
            var entiteitManager = new EntiteitManager();
            return View(entiteitManager.GetPerson(id));
        }

        public ActionResult Zoeken(string zoekwoord)
        {
            var entiteitManager = new EntiteitManager();
            var entiteiten = entiteitManager.ZoekEntiteiten(zoekwoord);
            return View(entiteiten);
        }

        // move content    vanaf file = request.files[0] alles naar manager verplaatsen
        [HttpPost]
        public ActionResult Upload()
        {
            var entiteitManager = new EntiteitManager();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    string str = (new StreamReader(file.InputStream)).ReadToEnd();
                    List<Domain.Entiteit.Persoon> jsonEntiteiten = JsonConvert.DeserializeObject<List<Domain.Entiteit.Persoon>>(str);
                    foreach (var p in jsonEntiteiten)
                    {
                        p.PlatformId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
                    }
                    entiteitManager.ConvertJsonToEntiteit(jsonEntiteiten);
                }
            }
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }

        // move content    string[] split ... t.e.m. } verplaatsen naar manager dmv AddThema(t, woorden, uploadfile)
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AddThema(Thema t, string woorden, HttpPostedFileBase uploadFile)
        {
            var entiteitManager = new EntiteitManager();
            string[] split = woorden.Split(',');
            List<Sleutelwoord> sleutelWoorden = new List<Sleutelwoord>();
            foreach (var woord in split)
            {
                Sleutelwoord sleutelwoord = new Sleutelwoord();
                sleutelwoord.woord = woord;
                sleutelWoorden.Add(sleutelwoord);
            }
            t.PlatformId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
            entiteitManager.AddThema(t, sleutelWoorden, uploadFile);
            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }
        
        // this region is for displaying a certain theme
        #region
        public virtual ActionResult DisplayTheme(int entityId)
        {
            var entiteitManager = new EntiteitManager();
            var toDisplay = entiteitManager.GetThema(entityId);
            return View(toDisplay);
        }
        #endregion

        //to remove ?
        // This region is for adding a person to the database and persisting.
        #region
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult AddPerson(int platformId)
        {
            var entiteitManager = new EntiteitManager();
            List<SelectListItem> listBoxItems = new List<SelectListItem>();

            List<Organisatie> allOrganisations = entiteitManager.GetAllOrganisaties((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            foreach (Organisatie o in allOrganisations)
            {
                SelectListItem organisation = new SelectListItem()
                {
                    Text = o.Naam,
                    Value = o.EntiteitId.ToString(),

                };
                listBoxItems.Add(organisation);
            }

            PersoonVM startCreation = new PersoonVM()
            {
                platId = platformId,
                OrganisationChecks = new SelectedOrganisationVM
                {
                    Organisations = listBoxItems
                }
            };

            return View(startCreation);
        }

        // move content
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult AddPerson(PersoonVM pvm, IEnumerable<string> selectedOrganisations)
        {

            var entiteitManager = new EntiteitManager();

            if (!ModelState.IsValid)
            {
                return View();
            }

            Persoon addedPerson = new Persoon
            {
                LastName = pvm.Ln,
                Organisations = new List<Organisatie>(),
                FirstName = pvm.Fn,
                PlatformId = pvm.platId,
                Naam = pvm.Fn + " " + pvm.Ln
            };

            if (selectedOrganisations != null)
            {
                foreach (string oId in selectedOrganisations)
                {
                    addedPerson.Organisations.Add(entiteitManager.GetOrganisatie(Int32.Parse(oId)));
                }
            }

            HttpPostedFileBase file = null;

            foreach (HttpPostedFileBase hpfb in Request.Files)
            {
                file = hpfb;
            }

            entiteitManager.AddPerson(addedPerson, file);

            return RedirectToAction("AdminBeheerEntiteiten", "Account");

        }
        #endregion
  
        // move content
        // This region is for displaying a certain person, given that a certain entityId is given.
        #region
        public virtual ActionResult DisplayPerson(int entityId)
        {
            var entiteitManager = new EntiteitManager();
            var postManager = new PostManager();
            Persoon toDisplay = entiteitManager.GetPerson(entityId);
            List<Post> posts = toDisplay.Posts;

            List<Double> polariteitPositief = new List<double>();
            foreach (Post post in posts)
            {
                if (post.Sentiment.polariteit >= 0)
                {
                    double waarde = post.Sentiment.polariteit;
                    polariteitPositief.Add(waarde);
                } 

            }
          
            int polariteitNegatiefCount = posts.Count - polariteitPositief.Count;
            int totaal = posts.Count;
            int polariteitPositiefCount = polariteitPositief.Count;
            var personViewModel = new PersonViewModel()
            {
                Persoon = toDisplay,
                AantalPosts = totaal,
                AantalPositieve = polariteitPositiefCount,
                AantalNegatieve = polariteitNegatiefCount,
                AantalMentions = postManager.getAantalMentions(toDisplay),
                TopWords = postManager.getTopPersonWords(toDisplay)


            };

            return View(personViewModel);
        }
        #endregion

        // move content
        // This region will handle the updating of a certain person. After the update you will be redirected to the Display page of the updated person;
        #region
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdatePerson(int entityId)
        {
            var entiteitManager = new EntiteitManager();
            List<SelectListItem> listBoxItems = new List<SelectListItem>();

            List<Organisatie> allOrganisations = entiteitManager.GetAllOrganisaties((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            foreach (Organisatie o in allOrganisations)
            {
                SelectListItem organisation = new SelectListItem()
                {
                    Text = o.Naam,
                    Value = o.EntiteitId.ToString(),

                };
                listBoxItems.Add(organisation);
            }

            UpdatePersonVM UPVM = new UpdatePersonVM
            {
                RequestedPerson = entiteitManager.GetPerson(entityId),
                OrganisationChecks = new SelectedOrganisationVM
                {
                    Organisations = listBoxItems
                }
            };


            return View(UPVM);
        }

        public ActionResult EditPerson(Domain.Entiteit.Persoon persoon)
        {
            IEntiteitManager entiteitManager = new EntiteitManager();
            Persoon persoonToUpdate = entiteitManager.GetPerson(persoon.EntiteitId);
            Organisatie organisatie = entiteitManager.GetOrganisatie(Int32.Parse(persoon.Organisation));
            persoonToUpdate.Naam = persoon.Naam;
            persoonToUpdate.Organisations.Clear();
            persoonToUpdate.Organisations.Add(organisatie);
            persoonToUpdate.Organisation = organisatie.Naam;
            entiteitManager.UpdateEntiteit(persoonToUpdate);
            return RedirectToAction("AdminCp", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdatePerson(UpdatePersonVM editedPerson, IEnumerable<string> selectedOrganisations, HttpPostedFileBase uploadFile)
        {
            var entiteitManager = new EntiteitManager();

            if (selectedOrganisations != null)
            {
                entiteitManager.ChangePerson(editedPerson.RequestedPerson, selectedOrganisations, uploadFile);
            }
            else
            {
                entiteitManager.ChangePerson(editedPerson.RequestedPerson, uploadFile);
            }

            return Redirect("~/Account/AdminBeheerEntiteiten");
        }
        #endregion

        // This region will handle the deletion of a certain person
        #region
        public virtual ActionResult DeletePerson(int entityId)
        {
            var entiteitManager = new EntiteitManager();
            entiteitManager.RemovePerson(entityId);
            return RedirectToAction("Index");
        }
        #endregion

        public virtual ActionResult RetrieveImage(int id)
        {
            var entiteitManager = new EntiteitManager();
            byte[] cover = null;
            var entiteit = entiteitManager.GetPerson(id) ?? (Entiteit) entiteitManager.GetOrganisatie(id);

            if (entiteit.GetType() == typeof(Persoon))
            {
                cover = entiteitManager.GetPersonImageFromDataBase(id);
            }
            else if (entiteit.GetType() == typeof(Organisatie))
            {
                cover = entiteitManager.GetOrganisationImageFromDataBase(id);
            }

            return cover != null ? File(cover, "image/jpg") : null;
        }
        // move content
        // This region will add a newly created Organisatie object to the database and persist
        #region
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult AddOrganisation(int platformId)
        {
            var entiteitManager = new EntiteitManager();
            List<SelectListItem> listBoxItems = new List<SelectListItem>();

            List<Persoon> allPeople = entiteitManager.GetAllPeople((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            foreach (Persoon p in allPeople)
            {
                SelectListItem person = new SelectListItem()
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EntiteitId.ToString(),

                };
                listBoxItems.Add(person);
            }

            OrganisatieVM startCreation = new OrganisatieVM()
            {
                platId = platformId,
                PeopleChecks = new SelectedPeopleVM
                {
                    People = listBoxItems
                }
            };

            return View(startCreation);
        }

        // move content
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult AddOrganisation(OrganisatieVM newOrganisation, IEnumerable<string> selectedPeople)
        {
            var entiteitManager = new EntiteitManager();

            if (!ModelState.IsValid)
            {
                return View();
            }

            Organisatie addedOrganisation = new Organisatie
            {
                Naam = newOrganisation.Name,
                Leden = new List<Persoon>(),
                PlatformId = newOrganisation.platId,
                Gemeente = newOrganisation.Town
            };

            if (selectedPeople != null)
            {
                foreach (string pId in selectedPeople)
                {
                    addedOrganisation.Leden.Add(entiteitManager.GetPerson(Int32.Parse(pId)));
                }
            }

            HttpPostedFileBase file = Request.Files["ImageData"];

            entiteitManager.AddOrganisatie(addedOrganisation, file);

            return RedirectToAction("AdminBeheerEntiteiten", "Account");
        }
        #endregion

        // This region is for displaying a certain Organisatie object, given that a certain entityId is given.
        #region
        public virtual ActionResult DisplayOrganisation(int entityId)
        {
            var entiteitManager = new EntiteitManager();
            Organisatie toDisplay = entiteitManager.GetOrganisatie(entityId);
            return View(toDisplay);
        }
        #endregion

        // move content
        public ActionResult EditOrganisation(Domain.Entiteit.Organisatie organisatie)
        {
            IEntiteitManager entiteitManager = new EntiteitManager();
            Organisatie organisatieToUpdate = entiteitManager.GetOrganisatie(organisatie.EntiteitId);
            organisatieToUpdate.Naam = organisatie.Naam;
            entiteitManager.UpdateEntiteit(organisatieToUpdate);
            return RedirectToAction("AdminCp", "Account");
        }

        // This region will handle the updating of a certain Organisation. After the update you will be redirected to the Display page of the updated organisation;
        #region
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdateOrganisation(int entityId)
        {
            var entiteitManager = new EntiteitManager();
            List<SelectListItem> listBoxItems = new List<SelectListItem>();

            List<Persoon> allPeople = entiteitManager.GetAllPeople((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            foreach (Persoon p in allPeople)
            {
                SelectListItem person = new SelectListItem()
                {
                    Text = p.FirstName + " " + p.LastName,
                    Value = p.EntiteitId.ToString(),

                };
                listBoxItems.Add(person);
            }

            UpdateOrganisatieVM UOVM = new UpdateOrganisatieVM
            {
                RequestedOrganisatie = entiteitManager.GetOrganisatie(entityId),
                PeopleChecks = new SelectedPeopleVM
                {
                    People = listBoxItems
                }
            };

            return View(UOVM);
        }


        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult UpdateOrganisation(UpdateOrganisatieVM vm, IEnumerable<string> selectedPeople, HttpPostedFileBase uploadFile)
        {
            var entiteitManager = new EntiteitManager();
            if (selectedPeople != null)
            {
                entiteitManager.ChangeOrganisatie(vm.RequestedOrganisatie, selectedPeople, uploadFile);

            }
            else
            {
                entiteitManager.ChangeOrganisatie(vm.RequestedOrganisatie, uploadFile);
            }
        
            return Redirect("~/Account/AdminBeheerEntiteiten");
        }
        #endregion

        // This region will handle the deletion of a certain Organisation
        #region
        public virtual ActionResult DeleteOrganisation(int entityId)
        {
            var entiteitManager = new EntiteitManager();
            entiteitManager.RemoveOrganisatie(entityId);

            return RedirectToAction("Index");
        }

        #endregion
        [Authorize(Roles = "SuperAdmin, Admin")]
        public void CreateTestData()
        {
            var entiteitManager = new EntiteitManager();
            entiteitManager.CreateTestData();
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult IndexThema()
        {
            var entiteitManager = new EntiteitManager();
            var themas = entiteitManager.GetThemas((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            return View(themas);
        }

        // GET: Thema/Create
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult CreateThema(int platid)
        {
            return View(new Thema { PlatformId = platid });
        }

        // move content
        // POST: Thema/Create
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult CreateThema(Thema thema, List<Sleutelwoord> sleutelwoorden, HttpPostedFileBase uploadFile)
        {
            var entiteitManager = new EntiteitManager();
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
                entiteitManager.AddThema(thema, mijnList, uploadFile);
                return RedirectToAction("IndexThema");
            }
            return View();
        }


        // GET: Thema/Edit/
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult EditThema(int id)
        {
            var entiteitManager = new EntiteitManager();
            return View(entiteitManager.GetThema(id));
        }

        // move content
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult EditThema(Thema thema, List<Sleutelwoord> sleutelwoorden, HttpPostedFileBase uploadFile)
        {

            var entiteitManager = new EntiteitManager();
            var mijnThema = entiteitManager.GetThema(thema.EntiteitId);
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
            entiteitManager.UpdateThema(thema, uploadFile);
            return Redirect("~/Account/AdminBeheerEntiteiten");
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThema(int id)
        {
            var entiteitManager = new EntiteitManager();
            TempData["themaID"] = entiteitManager.GetThema(id);
            return View(entiteitManager.GetThema(id));
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThema(int id,  FormCollection collection)
        {
            var entiteitManager = new EntiteitManager();
            entiteitManager.DeleteThema(id);
            return RedirectToAction("IndexThema");                                        
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThemaSleutelwoord(int id, int themaId)
        {
            TempData["themaID"] = themaId ;
            var entiteitManager = new EntiteitManager();
            entiteitManager.DeleteSleutelwoord(id);
            var thema = entiteitManager.GetThema(themaId);
            return View("~/Views/Entiteit/EditThema.cshtml", thema);
        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public virtual ActionResult DeleteThemaSleutelwoord(int id, int themaId, FormCollection collection)
        {

            return RedirectToAction("IndexThema");
            // return View();
        }

        public ActionResult ZoekEntiteit(string naam)
        {
            var entiteitManager = new EntiteitManager();
            var entiteiten = entiteitManager.GetEntiteiten(naam);
            TempData["MyList"] = entiteiten.ToList();
            return RedirectToAction("ShowEntiteiten");
        }

        // move content
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


        private Dictionary<Entiteit,string>  FillOrganisaties()
        {
            ArrayList organisaties = new ArrayList();
            Dictionary<Entiteit, string> naamType = new Dictionary<Entiteit, string>();

            var entiteitManager = new EntiteitManager();
            var entiteits = entiteitManager.GetAlleEntiteiten();
            if (naamType.Count == 0)
            {
                foreach (var entiteit in entiteits)
                {

                    if (entiteit is Organisatie)
                    {
                        naamType.Add(entiteit, "Organisatie");
                    }

                }
            }
            naamType.ToList().ForEach(x => organisaties.Add(x.Key.Naam));
            ViewBag.Organisaties = organisaties;
            return naamType;
        }
   
    }
}