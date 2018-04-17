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
    {    
       // GET: Entiteit
        public ActionResult Index()
        {
            return View();
        }    
    }
}