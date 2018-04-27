using BL;
using Domain.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class PlatformController : Controller
    {
        // GET: Platform
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportUsers()
        {
            IAccountManager accountManager = new AccountManager();
            List<Account> accounts = accountManager.GetAccounts();         
            return View(accounts);
        }

        public FileResult DownloadReport()
        {
            IPlatformManager platformManager = new PlatformManager();
            IAccountManager accountManager = new AccountManager();
            List<Account> list = accountManager.GetAccounts();
            StringBuilder sb = platformManager.ConvertToCSV(list);
            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv", "export.csv");
        }
    }
}