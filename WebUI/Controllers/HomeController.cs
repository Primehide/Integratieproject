
﻿using Domain.Platform;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;

namespace WebUI.Controllers
{
    [RequireHttps]
    public partial class HomeController : Controller
    {

        public ActionResult HomePagina()
        {
            return View();
        }
       

        public virtual ActionResult Index(string gekozenplatform,string tagline)

        {
            try
            {
                ViewBag.platId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
                ViewBag.dpnaam = gekozenplatform;
                ViewBag.tagline = tagline;
                return View();
            } catch ( NullReferenceException e )
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