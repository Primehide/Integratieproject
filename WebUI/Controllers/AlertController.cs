using BL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{


    public class AlertController : Controller
    {



        //GET
        public ActionResult AddAlert()
        {
            fillNamen();
            return View();

        }

        [HttpPost]
        public ActionResult AddAlert(Alert alert, bool android, bool web, bool mail, string types)
        {

            Entiteit entiteit = new Entiteit();
            Account acc = new Account();
            AccountManager acm = new AccountManager();
            EntiteitManager em = new EntiteitManager();
            acc = acm.getAccount("a48108d0-d944-4bcd-8f47-2ff7eee6d3f3");

            Alert newAlert = new Alert();
            string test = types;
            newAlert = alert;
            newAlert.Account = acc;
            fillNamen();

            entiteit = NaamType.Keys.Where(x => x.Naam == types).FirstOrDefault();
            newAlert.Entiteit = entiteit;


            if (android == true)
            {
                newAlert.PlatformType = PlatformType.ANDROID;
                acm.AddAlert(newAlert);

            }

            if (web == true)
            {
                newAlert.PlatformType = PlatformType.WEB;
                acm.AddAlert(newAlert);

            }

            if (mail == true)
            {
                newAlert.PlatformType = PlatformType.EMAIL;
                acm.AddAlert(newAlert);

            }

            fillNamen();
            return View();
        }
        Dictionary<Entiteit, string> NaamType = new Dictionary<Entiteit, string>();
        private void fillNamen()
        {
            
            ArrayList namen = new ArrayList();

            List<Entiteit> entiteits = new List<Entiteit>();

            EntiteitManager mgr = new EntiteitManager();
            entiteits = mgr.getAlleEntiteiten();

            foreach (Entiteit entiteit in entiteits)
            {
                if (entiteit is Persoon)
                {
                    NaamType.Add(entiteit, "Persoon");
                }
                if (entiteit is Organisatie)
                {
                    NaamType.Add(entiteit, "Organisatie");
                }
                if (entiteit is Thema)
                {
                    NaamType.Add(entiteit, "Thema");
                }



            }
            NaamType.ToList().ForEach(x => namen.Add(x.Key.Naam));
            ViewBag.Namen = namen;
        }
    }
}
