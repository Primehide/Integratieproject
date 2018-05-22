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

        

        public ActionResult berekenVasteGrafieken()
        {
            IPostManager postManager = new PostManager();
            postManager.maakVasteGrafieken();
            return View();
        }
    }


}