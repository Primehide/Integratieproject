using BL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Platform;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
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

        //Alleen de Superadmin meg een Platform Creëren
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult CreatePlatform()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult CreatePlatform(Deelplatform dp, HttpPostedFileBase imgLogo)
        {
            //I have to be able to create a SubPlatform

            _pM.AddDeelplatform(dp, imgLogo);
            EntiteitManager entiteitManager = new EntiteitManager();

            if (Request.Files.Count > 0)
            {
                entiteitManager.FileToJson(Request.Files[0], dp.DeelplatformId);
            }
            return RedirectToAction("Index");
        }
        
        //De volgende twee methodes zorgt voor het promoveren/demoveren van een user naar/van een Admin
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult PromoteAdmin(string identityId)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(store);
            manager.AddToRole(identityId, "Admin");
            return RedirectToAction("SuperAdminCp", "Account");
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DemoteAdmin(string identityId)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var store = new UserStore<ApplicationUser>(context);
            var manager = new ApplicationUserManager(store);
            manager.RemoveFromRole(identityId, "Admin");
            return RedirectToAction("SuperAdminCp", "Account");
        }

        //Het veranderen van Platform Eigenschappen wordt behandelt door deze methodes
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

        

        //Het binnenkomen van een deelplatform
        public ActionResult DisplayPlatform(int id)
        {

            //I have to be able to direct the user to the homepage of the selected Platform (Implementation)
            System.Web.HttpContext.Current.Session["PlatformID"] = id;
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Deelplatform p = _pM.GetDeelplatform(id);
            return RedirectToAction("Index", "Home", new { gekozenplatform = p.Naam, tagline = p.Tagline });
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;
            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }

        //Het verwijderen van een Deelplatform
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DeletePlatform(Deelplatform dp)
        {
            //All the entities that are related to the SubPlatform and the SubPlatform itself get deleted.
            _eM.DeleteEntiteitenVanDeelplatform(dp.DeelplatformId);
            _pM.RemoveDeelplatform(dp.DeelplatformId);
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult ExportUsers()
        {
            IAccountManager accountManager = new AccountManager();
            List<Account> accounts = accountManager.GetAccounts();
            return View(accounts);
        }

        //Admins en SuperAdmins moeten een Report kunnen downloaden
        [Authorize(Roles = "SuperAdmin, Admin")]
        public FileResult DownloadReport()
        {
            IPlatformManager platformManager = new PlatformManager();
            IAccountManager accountManager = new AccountManager();
            List<Account> list = accountManager.GetAccounts();
            StringBuilder sb = platformManager.ConvertToCSV(list);
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");
        }
    }
}