using BL;
using Domain.Entiteit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Entiteit;
using System.Data.Entity;

namespace WebUI.Controllers
{
    public class EntiteitController : Controller
<<<<<<< Updated upstream
    {    
       // GET: Entiteit
        public ActionResult Index()
        {
            return View();
        }    
=======
    {

        private IEntiteitManager mgr = new EntiteitManager();
        // GET: Entiteit
        public ActionResult IndexThema()
        {
            IEnumerable<Thema> themas = mgr.GetThemas();
            return View(themas);
        }

        // GET: Thema/Create
        public ActionResult CreateThema()
        {
            return View();
        }
        // POST: Thema/Create
        [HttpPost]
        public ActionResult CreateThema(Thema thema)
        {

            if (ModelState.IsValid)
            {

                mgr.AddThema(thema.Naam, thema.Trends, thema.Posts, thema.SleutenWoorden);
            }
            return View();
        }


        // GET: Thema/Edit/
        public ActionResult EditThema(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditThema(Thema thema)
        {
            if (ModelState.IsValid)
            {
                mgr.UpdateThema(thema);
                return RedirectToAction("Index");
            }
            return View();
        }


        public ActionResult DeleteThema(int id)
        {
            return View(mgr.GetThema(id));
        }

        [HttpPost]
        public ActionResult DeleteThema(int id, FormCollection collection)
        {
            try
            {
                mgr.DeleteThema(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
>>>>>>> Stashed changes
    }
}