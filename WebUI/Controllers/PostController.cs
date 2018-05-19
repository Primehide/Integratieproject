using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;
using Domain.Post;

namespace WebUI.Controllers
{
    public partial class PostController : Controller
    {
        // GET: Post
        public virtual ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async System.Threading.Tasks.Task SyncDataAsync()
        {
            IPostManager postManager = new PostManager();
            await postManager.SyncDataAsync();
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }

        [HttpPost]
        public ActionResult createGrafiek(WebUI.Models.GrafiekModel model)
        {
            IAccountManager accountManager = new AccountManager();
            List<CijferOpties> opties = new List<CijferOpties>();
            foreach (var optie in model.CijferOpties)
            {
                CijferOpties o = new CijferOpties()
                {
                    optie = optie
                };
                opties.Add(o);
            }
            //accountManager.grafiekAanGebruikerToevoegen(model.IdentityId,model.TypeGrafiek, model.EntiteitIds, model.CijferOpties, model.VergelijkOptie,model.GrafiekSoort);
            return RedirectToAction("Index","Manage");
        }

        public ActionResult berekenVasteGrafieken()
        {
            IPostManager postManager = new PostManager();
            postManager.maakVasteGrafieken();
            return View();
        }
    }


}