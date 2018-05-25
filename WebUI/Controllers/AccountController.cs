﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BL;
using Domain.Account;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebUI.Models;
using Domain.Entiteit;
using System.Collections;
using Domain.Post;
using Domain.Enum;
using Domain.Platform;

namespace WebUI.Controllers
{
    [Authorize]
    //[RequireHttps]
    public partial class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        //Code generated by identity
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? MakeSignInManager();
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
                return _userManager ?? MakeUserManager();
            }
            private set
            {
                _userManager = value;
            }
        }
        //

        public ApplicationSignInManager MakeSignInManager()
        {
            var uM = MakeUserManager();
            return new ApplicationSignInManager(uM, HttpContext.GetOwinContext().Authentication);
        }

        public ApplicationUserManager MakeUserManager()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var userstore = new ApplicationUserStore<ApplicationUser>(context);
            try
            {
                userstore.TenantId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
            }
            catch (NullReferenceException)
            {
                userstore.TenantId = 0;
            }
            ApplicationUserManager uM = new ApplicationUserManager(userstore);
            return uM;
        }

        [Authorize(Roles = "SuperAdmin")]
        public ActionResult SuperAdminCp()
        {
            IPlatformManager platformManager = new PlatformManager();
            List<Domain.Platform.Deelplatform> model = platformManager.GetAllDeelplatformen().ToList();
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AdminCp()
        {
            return View();
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AdminBeheerFaq()
        {
            IPlatformManager platformManager = new PlatformManager();
            IEnumerable<Faq> faqs = platformManager.GetAlleFaqs((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            return View(faqs);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult DeleteFaq(int id)
        {
            IPlatformManager platformManager = new PlatformManager();
            platformManager.DeleteFaq(id);
            return View("AdminBeheerFaq");
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AdminBeheerGebruikers()
        {
            AccountManager accountManager = new AccountManager();
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var userstore = new ApplicationUserStore<ApplicationUser>(context);
            List<Domain.Account.Account> domainUsers = new List<Account>();

            foreach (var item in userstore.GetAllUser())
            {
                if (item.TenantId == (int)System.Web.HttpContext.Current.Session["PlatformID"])
                {
                    domainUsers.Add(accountManager.getAccount(item.Id));
                }
            }

            AdminViewModel model = new AdminViewModel()
            {
                Users = domainUsers
            };
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AdminBeheerEntiteiten()
        {
            FillOrganisaties();
            EntiteitManager entiteitManager = new EntiteitManager();
            AdminViewModel model = new AdminViewModel()
            {
                PlatId = (int)System.Web.HttpContext.Current.Session["PlatformID"],
                AlleEntiteiten = entiteitManager.GetEntiteitenVanDeelplatform((int)System.Web.HttpContext.Current.Session["PlatformID"]),
            };
            return View(model);
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AddFaq(Faq f)
        {
            IPlatformManager platformManager = new PlatformManager();
            platformManager.AddFaq(f, (int)System.Web.HttpContext.Current.Session["PlatformID"]);
            return RedirectToAction("AdminBeheerFaq", "Account");
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            ApplicationUserStore<ApplicationUser> userstore = new ApplicationUserStore<ApplicationUser>(context);
            try
            {
                userstore.TenantId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
                ViewBag.ReturnUrl = returnUrl;
            }
            catch (System.NullReferenceException)
            {
                userstore.TenantId = 0;
                UserManager = new ApplicationUserManager(userstore);
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.platId = 0;
            }
            return View();
        }

        public async Task<bool> CheckIfAdminAsync(LoginViewModel model)
        {
            int oldplat = (int?)System.Web.HttpContext.Current.Session["PlatformID"] ?? 0;
            System.Web.HttpContext.Current.Session["PlatformID"] = 0;
            var adminUm = MakeUserManager();
            var user = await adminUm.FindByEmailAsync(model.Email);
            System.Web.HttpContext.Current.Session["PlatformID"] = oldplat;
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ViewResult> SetAdmin(string accountId)
        {
            var um = MakeUserManager();
            AccountManager accountManager = new AccountManager();
            Account a = accountManager.getAccount(accountId);
            a.IsAdmin = true;
            accountManager.updateUser(a);
            await um.AddToRoleAsync(accountId, "Admin");
            return View("EditUserAdmin", accountManager.getAccount(accountId));
        }

        public async Task<ViewResult> UnsetAdmin(string accountId)
        {
            var um = MakeUserManager();
            AccountManager am = new AccountManager();
            Account a = am.getAccount(accountId);
            a.IsAdmin = false;
            am.updateUser(a);
            await um.RemoveFromRoleAsync(accountId, "Admin");
            return View("EditUserAdmin", am.getAccount(accountId));
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var result = SignInStatus.Failure;
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Login", model);
            }

            // Require the user to have a confirmed email before they can log on.
            if (await CheckIfAdminAsync(model))
            {
                int oldplat = (int)System.Web.HttpContext.Current.Session["PlatformID"];
                System.Web.HttpContext.Current.Session["PlatformID"] = 0;
                ApplicationSignInManager asm = MakeSignInManager();
                result = await asm.PasswordSignInAsync(model.Email, model.Password, false, true);
                System.Web.HttpContext.Current.Session["PlatformID"] = oldplat;
            }
            else
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    if (!await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account-Resend");
                        ViewBag.errorMessage = "You must have a confirmed email to log on.";
                        return View("Error");
                    }
                }
                result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            }

            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View("Login", model);
            }
        }

        [AllowAnonymous]
        public ActionResult MakePartial()
        {
            if (System.Web.HttpContext.Current.Session["PlatformID"] == null)
            {
                ViewBag.platId = 0;
            }
            else
            {
                ViewBag.platId = (int)System.Web.HttpContext.Current.Session["PlatformID"];
            }
            return PartialView("_LoginPartial");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public virtual ActionResult Register()
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext<ApplicationUser>>();
            var userstore = new ApplicationUserStore<ApplicationUser>(context) { TenantId = (int)System.Web.HttpContext.Current.Session["PlatformID"] };
            UserManager = new ApplicationUserManager(userstore);
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, TenantId = (int)System.Web.HttpContext.Current.Session["PlatformID"] };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    IAccountManager accountManager = new AccountManager();
                    accountManager.CreateDomainUser(user.Id, user.Email, model.voornaam, model.achternaam, model.geboortedatum);
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    string callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");
                    //await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


                    return View("PleaseConfirmEmail");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View("Register", model);
        }

        //USER GRAFIEKEN
        [HttpGet]
        public ActionResult EditGrafiek(int id)
        {
            PostManager postManager = new PostManager();
            EntiteitManager entiteitManager = new EntiteitManager();
            List<Entiteit> alleEntiteiten = entiteitManager.getAlleEntiteiten(false);
            WebUI.Models.GrafiekViewModel model = new GrafiekViewModel()
            {
                Grafiek = postManager.GetGrafiek(id),
                Personen = alleEntiteiten.OfType<Persoon>().Where(x => x.PlatformId == (int)System.Web.HttpContext.Current.Session["PlatformID"]).ToList(),
                Organisaties = alleEntiteiten.OfType<Organisatie>().Where(x => x.PlatformId == (int)System.Web.HttpContext.Current.Session["PlatformID"]).ToList(),
                Themas = alleEntiteiten.OfType<Thema>().Where(x => x.PlatformId == (int)System.Web.HttpContext.Current.Session["PlatformID"]).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditGrafiek(Domain.Post.Grafiek grafiek, List<int> entiteitIds)
        {
            IPostManager postManager = new PostManager();
            postManager.UpdateGrafiek(entiteitIds, grafiek);
            return RedirectToAction("Index", "Manage");
        }

        [HttpPost]
        public ActionResult CreateGrafiek(GrafiekModel model)
        {
            IAccountManager accountManager = new AccountManager();
            List<CijferOpties> opties = accountManager.CreateCijferOpties(model.CijferOpties);
            accountManager.AddUserGrafiek(opties, model.EntiteitIds, model.TypeGrafiek, (int)System.Web.HttpContext.Current.Session["PlatformID"], User.Identity.GetUserId(), model.Naam, model.GrafiekSoort);
            return RedirectToAction("Index", "Manage");
        }
        //END USER GRAFIEKEN

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public virtual async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        public virtual ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public virtual ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = await UserManager.FindByNameAsync(model.Email /* + PlatformController.currentPlatform */);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            Session["Workaround"] = 0;
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        public virtual async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, TenantId = (int)System.Web.HttpContext.Current.Session["PlatformID"] };

                var result = await UserManager.CreateAsync(user);
                IAccountManager accountManager = new AccountManager();
                accountManager.CreateDomainUser(user.Id, user.Email, "Voornaam", "Achternaam", DateTime.Now);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public virtual ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public virtual ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        public virtual ActionResult GenereerAlerts()
        {
            BL.AccountManager accountManager = new BL.AccountManager();
            accountManager.genereerAlerts();
            return new HttpStatusCodeResult(200);
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        private async Task<string> SendEmailConfirmationTokenAsync(string userID, string subject)
        {
            string code = await UserManager.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ConfirmEmail", "Account",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await UserManager.SendEmailAsync(userID, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }
        #endregion

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult EditUserAdmin(string id)
        {
            AccountManager accountManager = new AccountManager();
            Account model = accountManager.getAccount(id);
            return View(model);
        }
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AdminDeleteEntiteit(int id)
        {
            EntiteitManager eM = new EntiteitManager();
            Entiteit entiteit = eM.GetEntiteit(id);
            switch (entiteit.GetType().Name)
            {
                case "Organisatie":
                    eM.RemoveOrganisatie(id);
                    break;
                case "Persoon":
                    eM.RemovePerson(id);
                    break;
                case "Thema":
                    eM.DeleteThema(id);
                    break;
            }

            return Redirect("~/Account/AdminBeheerEntiteiten");
        }

        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult AdminEditEntiteiten(int id)
        {
            EntiteitManager entiteitManager = new EntiteitManager();
            Entiteit entiteit = entiteitManager.GetEntiteit(id);
            if (entiteit.GetType() == typeof(Domain.Entiteit.Persoon))
            {
                Persoon persoon = entiteitManager.GetPerson(id);
                WebUI.Models.EditPersonViewModel model = new EditPersonViewModel()
                {
                    Persoon = persoon,
                    Organisaties = entiteitManager.GetAllOrganisaties((int)System.Web.HttpContext.Current.Session["PlatformID"])
                };
                return View("~/Views/Entiteit/EditPerson.cshtml", model);
            }
            else if (entiteit.GetType() == typeof(Domain.Entiteit.Organisatie))
            {
                Organisatie organisatie = entiteitManager.GetOrganisatie(id);
                return View("~/Views/Entiteit/UpdateOrganisation.cshtml", organisatie);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult EditUserAdmin(Domain.Account.Account model)
        {
            AccountManager accountManager = new AccountManager();
            accountManager.updateUser(model);
            return RedirectToAction("AdminBeheerGebruikers");
        }


        [Authorize(Roles = "SuperAdmin, Admin")]
        public ActionResult DeleteUser(string id)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.DeleteUser(id);
            UserManager.Delete(UserManager.FindById(id));
            return RedirectToAction("Index", "Home");
        }

        //push notifications
        // GET: Notification
        public ActionResult Alerts()
        {
            return View();
        }

        public JsonResult GetNotification()
        {
            return Json(NotificaionService.GetNotification(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FollowEntiteit(int id)
        {
            string entityId = User.Identity.GetUserId();
            IAccountManager accountManager = new AccountManager();
            accountManager.FollowEntity(entityId, id);
            return RedirectToAction("VolgItems", "Manage");
        }

        public ActionResult UnfollowEntiteit(int id)
        {
            string entityId = User.Identity.GetUserId();
            IAccountManager accountManager = new AccountManager();

            accountManager.UnfollowEntity(entityId, id);
            return RedirectToAction("VolgItems", "Manage");
        }

        Dictionary<Entiteit, string> NaamType = new Dictionary<Entiteit, string>();
        private void FillOrganisaties()
        {
            ArrayList organisaties = new ArrayList();
            List<Entiteit> entiteits = new List<Entiteit>();
            EntiteitManager mgr = new EntiteitManager();
            entiteits = mgr.GetEntiteitenVanDeelplatform((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            if (NaamType.Count == 0)
            {
                foreach (Entiteit entiteit in entiteits)
                {

                    if (entiteit is Organisatie)
                    {
                        NaamType.Add(entiteit, "Organisatie");
                    }
                }
            }
            NaamType.ToList().ForEach(x => organisaties.Add(x.Key.Naam));
            ViewBag.Organisaties = organisaties;
        }

        public ActionResult DeleteDashboardBlok(int id)
        {
            IAccountManager accountManager = new AccountManager();
            var account = accountManager.getAccount(User.Identity.GetUserId());
            accountManager.DeleteDashboardBlok(account, id);
            return new EmptyResult();
        }

        public ActionResult UpdateLocatie(int blokId, int locatie)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.UpdateLocatie(blokId, locatie);
            return new EmptyResult();
        }

        public ActionResult UpdateSize(int blokId, BlokGrootte blokGrootte)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.UpdateSize(blokId, blokGrootte);
            return new EmptyResult();
        }

        public ActionResult UpdateTitel(int blokId, String titel)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.UpdateTitel(blokId, titel);
            return new EmptyResult();
        }

        public ActionResult UpdateSizeDimensions(int blokId, int sizeX, int sizeY)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.UpdateSizeDimensions(blokId, sizeX, sizeY);
            return new EmptyResult();
        }

        public ActionResult UpdateConfiguratieTitle(int configuratieId, String title)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.UpdateConfiguratieTitle(configuratieId, title);
            return new EmptyResult();
        }

        public ActionResult SetPublic(int dashboardId, bool shared)
        {
            IAccountManager accountManager = new AccountManager();
            accountManager.SetPublic(dashboardId, shared);
            return new EmptyResult();
        }
    }
}