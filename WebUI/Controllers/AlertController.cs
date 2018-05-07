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


        public ActionResult Notifications()
        {
            IAccountManager accountManager = new AccountManager();
            List<Alert> alerts = accountManager.getAlleAlerts();

            IEnumerable<Alert> newalerts = alerts.Where(x => x.Account.IdentityId == User.Identity.GetUserId() && x.Triggered == true);

            return PartialView("~/Views/Shared/Dashboard/_DashboardAlerts.cshtml", newalerts);


        }

    }
}
