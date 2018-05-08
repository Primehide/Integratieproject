using BL;
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
        public String Get()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            string userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return userId;

        }

        //Change on which platform you are
        //return HttpStatusCode.OK if success
        [HttpPost]
        public string PostPlatform([FromBody] string username)
        {

            return username;

        }

    }
}
