using BL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Platform;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{

    public class PlatformController : Controller
    {
        readonly PlatformManager _pM = new PlatformManager();
        readonly EntiteitManager _eM = new EntiteitManager();


        // GET: Platform
        public ActionResult Index()
        {
            //I have to be able to see a list of created SubPlatforms
            return View(_pM.GetAllDeelplatformen());
        }

        //Creation of a SubPlatform (SuperAdmin)
        #region
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult CreatePlatform()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PromoteAdmin(string identityId)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(store);
            manager.AddToRole(identityId, "Admin");
            return RedirectToAction("SuperAdminCp", "Account");
        }

        [HttpPost]
        public ActionResult DemoteAdmin(string identityId)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(store);
            manager.RemoveFromRole(identityId, "Admin");
            return RedirectToAction("SuperAdminCp", "Account");
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult EditPlatform(int id)
        {
            IPlatformManager platformManager = new PlatformManager();
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var userstore = new ApplicationUserStore<ApplicationUser>(context);
            var allIdentity = new List<ApplicationUser>();

            foreach (var item in userstore.GetAllUser())
            {
                if(item.TenantId == id)
                {
                    allIdentity.Add(item);
                }
            }

            PlatformAdminModel model = new PlatformAdminModel()
            {
                Deelplatform = platformManager.GetDeelplatform(id),
                Users = allIdentity
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult EditPlatform(Deelplatform deelplatform, HttpPostedFileBase imgLogo)
        {
            IPlatformManager platformManager = new PlatformManager();
            platformManager.ChangeDeelplatform(deelplatform,imgLogo);
            return RedirectToAction("SuperAdminCp", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult CreatePlatform(Deelplatform dp, HttpPostedFileBase imgLogo)
        {
            //I have to be able to create a SubPlatform
            if (imgLogo != null)
            {
                BinaryReader reader = new BinaryReader(imgLogo.InputStream);
                var imageBytes = reader.ReadBytes(imgLogo.ContentLength);
                dp.Logo = imageBytes;
            }
            else
            {
                byte[] imageBytes = System.IO.File.ReadAllBytes("C:/Users/WaffleDealer/Desktop/IP/Integratieproject/WebUI/Controllers/default.png");
                dp.Logo = imageBytes;
            }

            _pM.AddDeelplatform(dp);
            EntiteitManager entiteitManager = new EntiteitManager();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    string str = (new StreamReader(file.InputStream)).ReadToEnd();
                    List<Persoon> jsonEntiteiten = JsonConvert.DeserializeObject<List<Persoon>>(str);
                    foreach (var p in jsonEntiteiten)
                    {
                        p.PlatformId = dp.DeelplatformId;
                    }
                    entiteitManager.ConvertJsonToEntiteit(jsonEntiteiten);
                }
            }
            return RedirectToAction("Index");
        }
        #endregion
        //Changing of a SubPlatform (Admin)
        #region

        [Authorize(Roles = "SuperAdmin")]
        public ActionResult ChangePlatform(int id)
        {
            List<Persoon> deelplatformPersonen = new List<Persoon>();
            List<Organisatie> deelplatformOrganisaties = new List<Organisatie>();
            List<Thema> deelplatformThemas = new List<Thema>();

            
            foreach (Entiteit e in _eM.GetEntiteitenVanDeelplatform(id))
            {
                if (e is Persoon persoon)
                {
                    deelplatformPersonen.Add(persoon);
                }
                else
                if (e is Organisatie organisatie)
                {
                    deelplatformOrganisaties.Add(organisatie);
                }
                else
                if (e is Thema thema)
                {
                    deelplatformThemas.Add(thema);
                }

            }

            ChangePlatformViewModel cpvm = new ChangePlatformViewModel
            {
                requestedDeelplatform = _pM.GetDeelplatform(id),
                personen = deelplatformPersonen,
                themas = deelplatformThemas,
                organisaties = deelplatformOrganisaties
            };

            return View(cpvm);

        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public ActionResult ChangePlatform(ChangePlatformViewModel dp)
        {
            //I have to be able to make new Entities that are related to the currently selected SubPlatform
            _pM.ChangeDeelplatform(dp.requestedDeelplatform);
            return RedirectToAction("Index");

        }

        #endregion
        //Display of a SubPlatform (Gebruiker)
        #region

        public ActionResult DisplayPlatform(int id)
        {
            //I have to be able to see the Entities that are related to the selected SubPlatform (Testing)
            #region
            //Deelplatform searchDeelplatform = pM.GetDeelplatform(id);

            //List<Persoon> deelplatformPersonen = new List<Persoon>();
            //List<Organisatie> deelplatformOrganisaties = new List<Organisatie>();
            //List<Thema> deelplatformThemas = new List<Thema>();

            //foreach (Entiteit e in eM.GetEntiteitenVanDeelplatform(id))
            //{
            //    if (e is Persoon)
            //    {
            //        deelplatformPersonen.Add((Persoon) e);
            //    } else
            //    if (e is Organisatie)
            //    {
            //        deelplatformOrganisaties.Add((Organisatie)e);
            //    } else 
            //    if (e is Thema)
            //    {
            //        deelplatformThemas.Add((Thema)e);
            //    }

            //}

            //ChangePlatformViewModel CPVM = new ChangePlatformViewModel
            //{
            //    requestedDeelplatform = pM.GetDeelplatform(id),
            //    personen = deelplatformPersonen,
            //    themas = deelplatformThemas,
            //    organisaties = deelplatformOrganisaties
            //};
            #endregion

            //I have to be able to direct the user to the homepage of the selected Platform (Implementation)
            #region

            System.Web.HttpContext.Current.Session["PlatformID"] = id;
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Deelplatform p = _pM.GetDeelplatform(id);
            return RedirectToAction("Index", "Home", new { gekozenplatform = p.Naam, tagline = p.Tagline });

            #endregion
            //return View(CPVM);

        }
        #endregion

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }

        //Deletion of a SubPlatform
        #region

        [HttpPost]
        public ActionResult DeletePlatform(Deelplatform dp)
        {
            //All the entities that are related to the SubPlatform and the SubPlatform itself get deleted.
            _eM.DeleteEntiteitenVanDeelplatform(dp.DeelplatformId);
            _pM.RemoveDeelplatform(dp.DeelplatformId);
            return RedirectToAction("Index");

        }

        #endregion
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult ExportUsers()
        {
            IAccountManager accountManager = new AccountManager();
            List<Account> accounts = accountManager.GetAccounts();
            return View(accounts);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public FileResult DownloadReport()
        {
            IPlatformManager platformManager = new PlatformManager();
            IAccountManager accountManager = new AccountManager();
            List<Account> list = accountManager.GetAccounts();
            StringBuilder sb = platformManager.ConvertToCsv(list);
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");
        }
    }
}