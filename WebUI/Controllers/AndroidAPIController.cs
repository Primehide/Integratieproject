using Domain.Account;
using Domain.Platform;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using BL.Managers;

namespace WebUI.Controllers
{
    public class AndroidAPIController : ApiController
    {
        PlatformManager _pM;
        AccountManager _aM;

        //Get all deelplatformen for android app
        [Route("api/deelplatformen")]
        [HttpGet]
        public Deelplatform[] GetDeelplatformen()
        {
            _pM = new PlatformManager();
            return _pM.GetAllDeelplatformen().ToArray();

        }
        //Get all DashboardBlokken (from specific user) for android app
        [Route("api/Blokken")]
        [HttpGet]
        [Authorize]
        public DashboardBlok[] Get()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;

            string userId = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            _aM = new AccountManager();

            var acc = _aM.GetAccount(userId);
            DashboardBlok[] dbs = acc.Dashboard.Configuratie.DashboardBlokken.ToArray();
            return dbs;
        }
    }
}
