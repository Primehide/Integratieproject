using BL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Platform;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    //TODO: REDIRECTS NAAR CHANGED PAGINA

    public class PlatformController : Controller
    {
        PlatformManager pM = new PlatformManager();
        EntiteitManager eM = new EntiteitManager();
        

        // GET: Platform
        public  ActionResult Index()
        {
            //I have to be able to see a list of created SubPlatforms


            return View(pM.GetAllDeelplatformen());
        }
        //Creation of a SubPlatform (SuperAdmin)
        #region
        public  ActionResult CreatePlatform()
        {
            return View();
        }

        [HttpPost]
        public  ActionResult CreatePlatform(Deelplatform dp)
        {
            //I have to be able to create a SubPlatform
            pM.AddDeelplatform(dp);
            return RedirectToAction("Index");
        }
        #endregion
        //Changing of a SubPlatform (Admin)
        #region

        [Authorize(Roles = "SuperAdmin, Admin")]
        public  ActionResult ChangePlatform(int id)
        {
            List<Persoon> deelplatformPersonen = new List<Persoon>();
            List<Organisatie> deelplatformOrganisaties = new List<Organisatie>();
            List<Thema> deelplatformThemas = new List<Thema>();

            List<Entiteit> Alleents = eM.GetEntiteitenVanDeelplatform(id);

            foreach (Entiteit e in eM.GetEntiteitenVanDeelplatform(id))
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

            ChangePlatformViewModel CPVM = new ChangePlatformViewModel
            {
                requestedDeelplatform = pM.GetDeelplatform(id),
                personen = deelplatformPersonen,
                themas = deelplatformThemas,
                organisaties = deelplatformOrganisaties
            };

            return View(CPVM);

        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        [HttpPost]
        public  ActionResult ChangePlatform(ChangePlatformViewModel dp)
        {
            //I have to be able to make new Entities that are related to the currently selected SubPlatform
            pM.ChangeDeelplatform(dp.requestedDeelplatform);
            return RedirectToAction("Index");

        }
        #endregion
        //Display of a SubPlatform (Gebruiker)
        #region
        public  ActionResult DisplayPlatform(int id)
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
            Deelplatform p = pM.GetDeelplatform(id);
            return RedirectToAction("Index", "Home",new { gekozenplatform = p.Naam, tagline = p.Tagline });

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
        public  ActionResult DeletePlatform()
        {
            return View();

        }

        [HttpPost]
        public  ActionResult DeletePlatform(Deelplatform dp)
        {
            //All the entities that are related to the SubPlatform and the SubPlatform itself get deleted.
            eM.DeleteEntiteitenVanDeelplatform(dp.DeelplatformId);
            pM.RemoveDeelplatform(dp.DeelplatformId);
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
            StringBuilder sb = platformManager.ConvertToCSV(list);
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");
        }
    }
}