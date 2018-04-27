using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    [RequireHttps]
    public partial class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            ViewBag.platId = PlatformController.currentPlatform;

            return View();
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