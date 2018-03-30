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

        // POST: Persoon
        public ActionResult AddPersoon(string Voornaam, string Achternaam, int OrganisatiePositie)
        {

        }

        // GET: Persoon
        public ActionResult DisplayPersoon(int PersoonId)
        {

        }

        // PUT: Persoon
        public ActionResult UpdatePersoon(int PersoonId)
        {

        }


    }
}