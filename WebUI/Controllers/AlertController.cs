using BL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using System.Web.Script.Serialization;
using WebUI.Models;

namespace WebUI.Controllers
{


    public class AlertController : Controller
    {
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }


        public ActionResult Notifications()
        {
            IAccountManager accountManager = new AccountManager();
            List<Alert> allAlerts = accountManager.getAlleAlerts();
            List<Alert> webalerts = new List<Alert>();
            allAlerts = allAlerts.Where(x => x.Triggered == true && x.Account.IdentityId == User.Identity.GetUserId()).ToList();
            webalerts = allAlerts.Where(x => x.PlatformType == PlatformType.WEB).ToList();
            return PartialView("~/Views/Shared/Dashboard/_DashboardAlerts.cshtml", webalerts.AsEnumerable());
        }

        

    }
}



