using BL;
using System;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    //[RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Faq()
        {
            var mgr = new AccountManager();
            var faqs = mgr.getAlleFaqs((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            return View(faqs);
        }

        public ActionResult HomePagina()
        {
            return View();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }

        public virtual ActionResult Index(string gekozenPlatform,string tagLine)

        {
            try
            {
                IPlatformManager platformManager = new PlatformManager();
                var platId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
                ViewBag.platId = platId;
                ViewBag.dpNaam = gekozenPlatform;
                ViewBag.tagLine = tagLine;

                return View(platformManager.GetDeelplatform(platId));
            } catch ( NullReferenceException )
            {
                return RedirectToAction("Index", "Platform", null);
            }
        }

        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public virtual ActionResult DashboardStart()
        {
            return View("~/Views/Shared/Dashboard/DashboardStarterKit.cshtml");
        }

    
    }
}