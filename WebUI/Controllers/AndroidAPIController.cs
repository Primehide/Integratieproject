using BL;
using Domain.Account;
using Domain.Platform;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using WebUI.Providers;

namespace WebUI.Controllers
{
    public class AndroidAPIController : ApiController
    {
        PlatformManager pM;
        AccountManager aM;

        //Get all deelplatformen for android app
        [Route("api/deelplatformen")]
        [HttpGet]
        public Deelplatform[] GetDeelplatformen()
        {
            pM = new PlatformManager();
            return pM.GetAllDeelplatformen().ToArray();

        }
        //Get all DashboardBlokken (from specific user) for android app
        [Route("api/Blokken")]
        [HttpGet]
        [Authorize]
        public DashboardBlok[] Get()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            string userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            aM = new AccountManager();

            var acc = aM.getAccount(userId);
            DashboardBlok[] dbs = acc.Dashboard.Configuratie.DashboardBlokken.ToArray();
            return dbs;
        }
    }
}
