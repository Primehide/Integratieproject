using BL;
using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class EntiteitController : Controller
    {
        private EntiteitManager eM = new EntiteitManager();

        // GET: Entiteit
        public ActionResult Index()
        {
            return View();
        }



        // POST: Persoon

        public ActionResult AddPerson()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPerson(PersoonVM pvm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Persoon AddedPerson = new Persoon
            {
                LastName = pvm.Ln,
                Organisations = new List<Organisatie>(),
                FirstName = pvm.Fn
            };
            eM.AddPerson(AddedPerson);
            return View("DisplayPerson", AddedPerson); 
        }



        // GET: Persoon

        public ActionResult DisplayPerson()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DisplayPerson(int PersonId)
        {
            Persoon ToDisplay = eM.GetPerson(PersonId);
            return View(ToDisplay);
        }

        // PUT: Persoon

        public ActionResult UpdatePerson()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdatePerson(Persoon EditedPerson)
        {
            eM.ChangePerson(EditedPerson);
            return View("DisplayPerson",EditedPerson);
        }

        [HttpPost]
        public ActionResult DeletePerson(int id)
        {
            eM.RemovePerson(id);
            return View();
        }

    }
}