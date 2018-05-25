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

        //Creation of a SubPlatform (SuperAdmin)

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
            
            _pM.AddDeelplatform(dp,imgLogo);
            EntiteitManager entiteitManager = new EntiteitManager();

            if (Request.Files.Count > 0)
            {
                entiteitManager.FileToJson(Request.Files[0], dp.DeelplatformId);
            }
            return RedirectToAction("Index");
        }

        //Display of a SubPlatform (Gebruiker)
        #region

        public ActionResult DisplayPlatform(int id)
        {

            //I have to be able to direct the user to the homepage of the selected Platform (Implementation)
            System.Web.HttpContext.Current.Session["PlatformID"] = id;
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Deelplatform p = _pM.GetDeelplatform(id);
            return RedirectToAction("Index", "Home", new { gekozenplatform = p.Naam, tagline = p.Tagline });
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