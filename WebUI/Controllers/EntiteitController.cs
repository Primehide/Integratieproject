using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class EntiteitController : Controller
    {
        // GET: Entiteit
        public ActionResult Index()
        {
            return View();
        }

        public void CreateTestData()
        {
            BL.EntiteitManager entiteitManager = new BL.EntiteitManager();
            entiteitManager.CreateTestData();
        }
    }
}