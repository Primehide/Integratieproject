using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;

namespace WebUI.Controllers
{
    public class PostController : Controller
    {
        // GET: Post
        public ActionResult Index()
        {
            return View();
        }

        public async System.Threading.Tasks.Task SyncDataAsync()
        {
            IPostManager postManager = new PostManager();
            await postManager.SyncDataAsync();
        }

        [HttpPost]
        public ActionResult createGrafiek(WebUI.Models.GrafiekModel model)
        {
            IAccountManager accountManager = new AccountManager();
            List<int> entiteitInts = new List<int>();
            //voeg later alle ints toe
            entiteitInts.Add(model.Entiteit1);

            accountManager.grafiekAanGebruikerToevoegen(model.IdentityId,model.TypeGrafiek, entiteitInts);
            return new HttpStatusCodeResult(200);
        }
    }
}