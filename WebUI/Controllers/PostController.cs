using System.Web.Mvc;
using BL.Interfaces;
using BL.Managers;

namespace WebUI.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        public virtual ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public async System.Threading.Tasks.Task SyncDataAsync()
        {
            //alle deelplatform, alle entiteiten updaten
            IPostManager postManager = new PostManager();
            IPlatformManager platformManager = new PlatformManager();
            foreach (var dp in platformManager.GetAllDeelplatformen())
            {
                await postManager.SyncDataAsync(dp.DeelplatformId);
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }

        public ActionResult BerekenVasteGrafieken()
        {
            IPostManager postManager = new PostManager();
            postManager.MaakVasteGrafieken();
            return View();
        }
    }


}