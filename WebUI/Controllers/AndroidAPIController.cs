using BL;
using Domain.Account;
using Domain.Platform;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace WebUI.Controllers
{
    //De API controller die zal aangesproken worden door de Android App
    public class AndroidApiController : ApiController
    {
        PlatformManager _pM;
        AccountManager _aM;

        //Vraagt alle aangemaakte deeplatformen op voor de Android app
        [Route("api/deelplatformen")]
        [HttpGet]
        public Deelplatform[] GetDeelplatformen()
        {
            _pM = new PlatformManager();
            return _pM.GetAllDeelplatformen().ToArray();

        }

        //Neemt de request van een ingelogde user, en zal deze gebruiken om een dashboard terug te sturen
        //Dit dashboard zal worden getekend op de app zelf
        [Route("api/Blokken")]
        [HttpGet]
        [Authorize]
        public DashboardBlok[] Get()
        {
            ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
            _aM = new AccountManager();
            return _aM.GetDashboardBloks(principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
