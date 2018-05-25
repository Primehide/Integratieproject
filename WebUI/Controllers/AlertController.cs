using BL;
using Domain.Enum;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web.Mvc;

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

            var webAlerts = accountManager
                .getAlleAlerts()
                .Where(x => x.Triggered && x.Account.IdentityId == User.Identity.GetUserId() && x.PlatformType == PlatformType.WEB)
                .ToList();

            return PartialView("~/Views/Shared/Dashboard/_DashboardAlerts.cshtml", webAlerts.AsEnumerable());
        }

    }
}



