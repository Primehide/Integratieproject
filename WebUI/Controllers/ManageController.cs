using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Authorize]
    public partial class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ActionResult VolgItems()
        {
            IAccountManager accountManager = new AccountManager();
            IEntiteitManager entiteitManager = new EntiteitManager();
            Models.DashboardModel model = new DashboardModel()
            {
                GevolgdeItems = accountManager.getAccount(User.Identity.GetUserId()).Items.ToList(),
                AlleEntiteiten = entiteitManager.getAlleEntiteiten()
            };
            return View(model);
        }

        
        // GET: /Manage/Index
        public ActionResult Index()
        {
            IAccountManager accountManager = new AccountManager();

            WebUI.Models.DashboardModel model = new DashboardModel()
            {
                Configuratie = accountManager.getAccount(User.Identity.GetUserId()).Dashboard.Configuratie,
                GrafiekLabels = new Dictionary<string, string>(),
                GrafiekDataSets = new Dictionary<string, string>(),
                ColorCodes = new List<string>(),
            };

            model.ColorCodes.Add("#2E2EFE");
            model.ColorCodes.Add("#74DF00");
            model.ColorCodes.Add("#BF00FF");
            model.ColorCodes.Add("#6E6E6E");
            model.ColorCodes.Add("#0489B1");
            model.ColorCodes.Add("#FE2E2E");
            model.ColorCodes.Add("#FF8000");
            model.ColorCodes.Add("#DA81F5");
            model.ColorCodes.Add("#FA5882");
            model.ColorCodes.Add("#0B6121");
            model.ColorCodes.Add("#4286f4");

            model.ColorCodes.Add("#7bce81");
            model.ColorCodes.Add("#2d9183");
            model.ColorCodes.Add("#99445c");
            model.ColorCodes.Add("#2c5b48");
            model.ColorCodes.Add("#b74909");
            model.ColorCodes.Add("#e2b663");
            model.ColorCodes.Add("#7c5f26");
            model.ColorCodes.Add("#e8a31e");
            model.ColorCodes.Add("#e2e08c");
            model.ColorCodes.Add("#7f7d2f");
            model.ColorCodes.Add("#d3ce10");

            model.ColorCodes.Add("#cbf2d2");
            model.ColorCodes.Add("#21dd43");
            model.ColorCodes.Add("#d6aeef");
            model.ColorCodes.Add("#8b0fd8");
            model.ColorCodes.Add("#dd0f95");

            int grafiekTeller = 0;
            int dataSetTeller = 0;
            //overlopen van elke blok
            foreach (var blok in model.Configuratie.DashboardBlokken.Where(x => x.Grafiek.Type != Domain.Enum.GrafiekType.CIJFERS))
            {
                //dataset teller resetten
                dataSetTeller = 0;
                //kijkt na of het soort gegeven een postfrequentie is. Als dat zo is zijn de labels anders.
                if (blok.Grafiek.soortGegevens == Domain.Enum.SoortGegevens.POSTFREQUENTIE)
                {
                    DateTime vandaag = DateTime.Today;
                    //Labels aanmaken van laatste 10 dagen
                    //post frequentie toont het aantal posts van vandaag tot 10 dagen terug
                    StringBuilder labelBuilder = new StringBuilder();
                    StringBuilder dataBuilder = new StringBuilder();
                    //Labels aanmaken van laatste 10 dagen
                    for (int i = 10; i > 0; i--)
                    {
                        labelBuilder.Append("'").Append(vandaag.AddDays(-i).Date.ToShortDateString()).Append("'").Append(",");
                    }
                    model.GrafiekLabels.Add("LabelsGrafiek " + grafiekTeller, labelBuilder.ToString());
                    //Elke waarde van de grafiek overlopen en toevoegen aan de dictonary
                    for (int i = 0; i < blok.Grafiek.Waardes.Count; i++)
                    {
                        if (blok.Grafiek.Waardes.ElementAt(i).Naam.ToLower().Contains("endpostfrequentie"))
                        {
                            model.GrafiekDataSets.Add("DataSetsGrafiek " + grafiekTeller + "DataSet " + dataSetTeller, dataBuilder.ToString());
                            dataSetTeller++;
                            dataBuilder.Clear();
                            continue;
                        }
                        dataBuilder.Append(blok.Grafiek.Waardes.ElementAt(i).Waarde).Append(",");
                    }

                }
                else if (blok.Grafiek.soortGegevens == Domain.Enum.SoortGegevens.POPULARITEIT)
                {
                    StringBuilder labelBuilder = new StringBuilder();
                    StringBuilder dataBuilder = new StringBuilder();
                    foreach (var waarde in blok.Grafiek.Waardes)
                    {
                        labelBuilder.Append("'").Append(waarde.Naam).Append("'").Append(",");
                        dataBuilder.Append(waarde.Waarde).Append(",");
                    }
                    model.GrafiekLabels.Add("LabelsGrafiek " + grafiekTeller, labelBuilder.ToString());
                    model.GrafiekDataSets.Add("DataSetsGrafiek " + grafiekTeller + "DataSet " + dataSetTeller, dataBuilder.ToString());
                }
                //grafiek is gemaakt, teller met 1 verhogen
                grafiekTeller++;
            }

            return View(model);
            //return View();
        }

        public ActionResult Visit(int id)
        {
            IAccountManager accountManager = new AccountManager();
            Account account = accountManager.getAccount(id);
            WebUI.Models.DashboardModel model = new DashboardModel()
            {
                IsPublic = account.Dashboard.IsPublic,
                Configuratie = account.Dashboard.Configuratie,
                GrafiekLabels = new Dictionary<string, string>(),
                GrafiekDataSets = new Dictionary<string, string>(),
                ColorCodes = new List<string>()
            };

            model.ColorCodes.Add("#2E2EFE");
            model.ColorCodes.Add("#74DF00");
            model.ColorCodes.Add("#BF00FF");
            model.ColorCodes.Add("#6E6E6E");
            model.ColorCodes.Add("#0489B1");
            model.ColorCodes.Add("#FE2E2E");
            model.ColorCodes.Add("#FF8000");
            model.ColorCodes.Add("#DA81F5");
            model.ColorCodes.Add("#FA5882");
            model.ColorCodes.Add("#0B6121");
            model.ColorCodes.Add("#4286f4");

            model.ColorCodes.Add("#7bce81");
            model.ColorCodes.Add("#2d9183");
            model.ColorCodes.Add("#99445c");
            model.ColorCodes.Add("#2c5b48");
            model.ColorCodes.Add("#b74909");
            model.ColorCodes.Add("#e2b663");
            model.ColorCodes.Add("#7c5f26");
            model.ColorCodes.Add("#e8a31e");
            model.ColorCodes.Add("#e2e08c");
            model.ColorCodes.Add("#7f7d2f");
            model.ColorCodes.Add("#d3ce10");

            model.ColorCodes.Add("#cbf2d2");
            model.ColorCodes.Add("#21dd43");
            model.ColorCodes.Add("#d6aeef");
            model.ColorCodes.Add("#8b0fd8");
            model.ColorCodes.Add("#dd0f95");


            int grafiekTeller = 0;
            int dataSetTeller = 0;
            //overlopen van elke blok
            foreach (var blok in model.Configuratie.DashboardBlokken.Where(x => x.Grafiek.Type != Domain.Enum.GrafiekType.CIJFERS))
            {
                //dataset teller resetten
                dataSetTeller = 0;
                //kijkt na of het soort gegeven een postfrequentie is. Als dat zo is zijn de labels anders.
                if (blok.Grafiek.soortGegevens == Domain.Enum.SoortGegevens.POSTFREQUENTIE)
                {
                    DateTime vandaag = DateTime.Today;
                    //Labels aanmaken van laatste 10 dagen
                    //post frequentie toont het aantal posts van vandaag tot 10 dagen terug
                    StringBuilder labelBuilder = new StringBuilder();
                    StringBuilder dataBuilder = new StringBuilder();
                    //Labels aanmaken van laatste 10 dagen
                    for (int i = 10; i > 0; i--)
                    {
                        labelBuilder.Append("'").Append(vandaag.AddDays(-i).Date.ToShortDateString()).Append("'").Append(",");
                    }
                    model.GrafiekLabels.Add("LabelsGrafiek " + grafiekTeller, labelBuilder.ToString());
                    //Elke waarde van de grafiek overlopen en toevoegen aan de dictonary
                    for (int i = 0; i < blok.Grafiek.Waardes.Count; i++)
                    {
                        if (blok.Grafiek.Waardes.ElementAt(i).Naam.ToLower().Contains("endpostfrequentie"))
                        {
                            model.GrafiekDataSets.Add("DataSetsGrafiek " + grafiekTeller + "DataSet " + dataSetTeller, dataBuilder.ToString());
                            dataSetTeller++;
                            dataBuilder.Clear();
                            continue;
                        }
                        dataBuilder.Append(blok.Grafiek.Waardes.ElementAt(i).Waarde).Append(",");
                    }

                }
                else if (blok.Grafiek.soortGegevens == Domain.Enum.SoortGegevens.POPULARITEIT)
                {
                    StringBuilder labelBuilder = new StringBuilder();
                    StringBuilder dataBuilder = new StringBuilder();
                    foreach (var waarde in blok.Grafiek.Waardes)
                    {
                        labelBuilder.Append("'").Append(waarde.Naam).Append("'").Append(",");
                        dataBuilder.Append(waarde.Waarde).Append(",");
                    }
                    model.GrafiekLabels.Add("LabelsGrafiek " + grafiekTeller, labelBuilder.ToString());
                    model.GrafiekDataSets.Add("DataSetsGrafiek " + grafiekTeller + "DataSet " + dataSetTeller, dataBuilder.ToString());
                }
                //grafiek is gemaakt, teller met 1 verhogen
                grafiekTeller++;
            }

            if (model.IsPublic)
            {
                return View(model);
            }
            else
            {
                return View();
            }

     
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        public ActionResult AddGrafiek()
        {
            IEntiteitManager entiteitManager = new EntiteitManager();
            List<Entiteit> AlleEntiteiten = entiteitManager.getAlleEntiteiten(false);
            IPlatformManager platformManager = new PlatformManager();
            var dp = platformManager.GetDeelplatform((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            WebUI.Models.GrafiekViewModel model = new GrafiekViewModel()
            {
                Personen = AlleEntiteiten.OfType<Persoon>().Where(x => x.PlatformId == (int)System.Web.HttpContext.Current.Session["PlatformID"]).ToList(),
                Organisaties = AlleEntiteiten.OfType<Organisatie>().Where(x => x.PlatformId == (int)System.Web.HttpContext.Current.Session["PlatformID"]).ToList(),
                Themas = AlleEntiteiten.OfType<Thema>().Where(x => x.PlatformId == (int)System.Web.HttpContext.Current.Session["PlatformID"]).ToList(),
            };
            return View(model);
        }



  

        //
        // GET: /Manage/ChangePassword
        public virtual ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public virtual ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public virtual async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public virtual async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        // Manage Profile

        // GET: Manage Profile
        public ActionResult UpdateProfile()
        {
            AccountManager acm = new AccountManager();
            Domain.Account.Account model = acm.getAccount(User.Identity.GetUserId());
            return View(model);
        }

        // Manage Alerts

        // GET
        public ActionResult UpdateAlerts()
        {
        
           FillEntiteiten();
            IAccountManager accountManager = new AccountManager();
            List<Alert> newAlerts = accountManager.GetUserAlerts(User.Identity.GetUserId());
            return View("UpdateAlerts", newAlerts);
        }

        //POST: Delete Alert
        public ActionResult DeleteAlert(int id)
        {
       
            FillEntiteiten();
            IAccountManager accountManager = new AccountManager();
            accountManager.DeleteAlert(id);
            List<Alert> newAlerts = accountManager.GetUserAlerts(User.Identity.GetUserId());

            return View("updatealerts", newAlerts);
        }
        //GET: Edit Alert
        public ActionResult EditAlert(int id)
        {
  
            FillEntiteiten();

            IAccountManager accountManager = new AccountManager();
            Alert alert = accountManager.GetAlert(id);
            AlertViewModel avm = new AlertViewModel();
            avm.alert = alert;
            return View(avm);
        }

        // POST: Alert/Edit/
        [HttpPost]
        public ActionResult EditAlert(int id, AlertViewModel modelalert)
        {

            IAccountManager accountManager = new AccountManager();
            if (ModelState.IsValid)
            {
                //change

             
                Entiteit entiteit = new Entiteit();
                entiteit = FillEntiteiten().Keys.Where(x => x.Naam == modelalert.type).FirstOrDefault();
                modelalert.alert.Entiteit = entiteit;
                modelalert. alert.AlertId = id;
                accountManager.UpdateAlert(modelalert.alert);

                List<Alert> newAlerts = accountManager.GetUserAlerts(User.Identity.GetUserId());


                return RedirectToAction("UpdateAlerts", newAlerts);
            }

            return View();
        }
   
        [HttpPost]
        public ActionResult AddAlert(AlertViewModel model)
        {
            AccountManager accountManager = new AccountManager();
            //entiteit ID ophalen
            int entiteitId = FillEntiteiten().Keys.Where(x => x.Naam == model.type).FirstOrDefault().EntiteitId;
  

            //Account 
            model.alert.Account = accountManager.getAccount(User.Identity.GetUserId());
           
            accountManager.AddAlert(model.alert, entiteitId, model.web, model.android, model.mail);
            List<Alert> newAlerts = accountManager.GetUserAlerts(User.Identity.GetUserId());

            return View("UpdateAlerts", newAlerts);

        }

       
      
        //POST /Manage/ManageAccount
        [HttpPost]
        public virtual async Task<ActionResult> ManageAccount(Models.ChangeProfileViewModel model)
        {
            AccountManager acm = new AccountManager();
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            Domain.Account.Account accountToUpdate = acm.getAccount(user.Id);

            accountToUpdate.Voornaam = model.Voornaam;
            accountToUpdate.Achternaam = model.Achternaam;
            accountToUpdate.GeboorteDatum = model.Geboortedatum;

            // if email changed:
            if (User.Identity.GetUserName() != model.Email)
            {
                user.EmailConfirmed = false;
                user.Email = model.Email;
                user.UserName = model.Email;
                accountToUpdate.Email = model.Email;

                //security stamp vernieuwen
                await UserManager.UpdateSecurityStampAsync(User.Identity.GetUserId());
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                //User updaten
                UserManager.Update(user);

                //send mail 
                var callbackUrl = Url.Action("ConfirmEmail", "Account",
                   new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "confirmation",
                   "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //sign out
                var AuthenticationManager = HttpContext.GetOwinContext().Authentication;
                AuthenticationManager.SignOut();

            }

            acm.UpdateUser(accountToUpdate);
            //ManageAccount();
            return new HttpStatusCodeResult(200);

        }


        public ActionResult GenereerReview()
        {
            IAccountManager accountManager = new AccountManager();
            List<Account> accounts = accountManager.GetAccounts();
            
            IEntiteitManager entiteitManager = new EntiteitManager();
            foreach (Account acc in accounts)
            {
                //Review aanmaken
                
                List<Entiteit> entiteiten = new List<Entiteit>();
                List<Item> items = acc.Items;
                foreach(Item item in items)
                {
                   
                    entiteiten.Add(entiteitManager.GetEntiteit(item.EntiteitId));
                }
             
                acc.ReviewEntiteiten = entiteiten;
                accountManager.UpdateUser(acc);
            }
            return View();
        }

        public ActionResult ShowReview()
        {
            string id = User.Identity.GetUserId();
            IAccountManager accountManager = new AccountManager();
            Account account = accountManager.getAccount(id);
            List<Entiteit> entiteiten = account.ReviewEntiteiten;
            List<Persoon> deelplatformPersonen = new List<Persoon>();
            List<Organisatie> deelplatformOrganisaties = new List<Organisatie>();
            List<Thema> deelplatformThemas = new List<Thema>();

            foreach (Entiteit e in entiteiten)
            {
                if (e is Persoon)
                {
                    deelplatformPersonen.Add((Persoon)e);
                }
                else
                if (e is Organisatie)
                {
                    deelplatformOrganisaties.Add((Organisatie)e);
                }
                else
                if (e is Thema)
                {
                    deelplatformThemas.Add((Thema)e);
                }
            }

            ReviewModel reviewModel = new ReviewModel()
            {
                Organisaties = deelplatformOrganisaties,
                Personen = deelplatformPersonen,
                Themas = deelplatformThemas
            };
            return View(reviewModel);
        }

        //Error page
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new ViewResult
            {
                ViewName = "~/Views/Shared/Error.cshtml"
            };
        }

        public Dictionary<Entiteit, string> FillEntiteiten()
        {
            Dictionary<Entiteit, string> NaamType = new Dictionary<Entiteit, string>();
            ArrayList namen = new ArrayList();

            List<Entiteit> entiteits = new List<Entiteit>();

            EntiteitManager mgr = new EntiteitManager();
            entiteits = mgr.GetEntiteitenVanDeelplatform((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            if (NaamType.Count == 0)
            {
                foreach (Entiteit entiteit in entiteits)
                {
                    if (entiteit is Persoon)
                    {
                        NaamType.Add(entiteit, "Persoon");
                    }
                    if (entiteit is Organisatie)
                    {
                        NaamType.Add(entiteit, "Organisatie");
                    }
                    if (entiteit is Thema)
                    {
                        NaamType.Add(entiteit, "Thema");
                    }



                }
            }
            NaamType.ToList().ForEach(x => namen.Add(x.Key.Naam));
            ViewBag.Namen = namen;

            return NaamType;
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }

}