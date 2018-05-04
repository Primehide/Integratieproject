using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;

namespace WebUI.Controllers
{
    public partial class PostController : Controller
    {
        // GET: Post
        public virtual ActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task SyncDataAsync(bool AllPosts = false)
        {
            IPostManager postManager = new PostManager();
            await postManager.SyncDataAsync(AllPosts);
        }

        [HttpPost]
        public ActionResult createGrafiek(WebUI.Models.GrafiekModel model)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.grafiekAanGebruikerToevoegen(model.IdentityId,model.TypeGrafiek, model.EntiteitIds, model.CijferOpties, model.VergelijkOptie,model.GrafiekSoort);
            return RedirectToAction("Index","Manage");
        }


    }


}