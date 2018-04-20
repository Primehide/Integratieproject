using BL;
using Domain.Entiteit;
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

        private IEntiteitManager mgr = new EntiteitManager();

        public void CreateTestData()
        {
            BL.EntiteitManager entiteitManager = new BL.EntiteitManager();
            entiteitManager.CreateTestData();
        }

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
        public ActionResult CreateThema(Thema thema, List<Sleutelwoord> sleutelwoorden)
        {
           // sleutelwoorden.RemoveAll(item => item.woord == null);
            string woorden = sleutelwoorden[0].woord;
            string[] split = woorden.Split(',');
            List<Sleutelwoord> mijnList = new List<Sleutelwoord>();
            foreach (string woord in split)
            {
                Sleutelwoord sleutelwoord = new Sleutelwoord(woord);
                mijnList.Add(sleutelwoord);
            }
            if (ModelState.IsValid)
            {
                mgr.AddThema(thema.Naam,mijnList);
                return RedirectToAction("IndexThema");
            }
            return View();
        }


        // GET: Thema/Edit/
        public ActionResult EditThema(int id)
        {
            return View(mgr.GetThema(id));
        }

        [HttpPost]
        public ActionResult EditThema(Thema thema, int id)
        {
            
            thema.EntiteitId = id;
                mgr.UpdateThema(thema);
                return RedirectToAction("IndexThema");                 
        }


        public ActionResult DeleteThema(int id)
        {
            return View(mgr.GetThema(id));
        }

        [HttpPost]
        public ActionResult DeleteThema(int id, FormCollection collection)
        {
           
            
                mgr.DeleteThema(id);
                return RedirectToAction("IndexThema");
            
            
            
               // return View();
            
        }
    }
}