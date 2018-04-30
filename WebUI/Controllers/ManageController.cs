using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BL;
using Domain.Account;
using Microsoft.AspNet.Identity;
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

        

        //
        // GET: /Manage/Index
        public ActionResult Index()
        {
            IAccountManager accountManager = new AccountManager();

            WebUI.Models.DashboardModel model = new DashboardModel()
            {
                Configuratie = accountManager.getAccount(User.Identity.GetUserId()).Dashboard.Configuratie,
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
                else if(blok.Grafiek.soortGegevens == Domain.Enum.SoortGegevens.POPULARITEIT)
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
            List<Domain.Entiteit.Persoon> personen = entiteitManager.GetAllPeople().ToList();
            WebUI.Models.GrafiekViewModel model = new GrafiekViewModel()
            {
                Personen = entiteitManager.GetAllPeople(),
                Organisaties = entiteitManager.GetAllOrganisaties(),
                Themas = entiteitManager.GetThemas().ToList()
            };
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Sandbox()
        {
            IEntiteitManager entiteitManager = new EntiteitManager();
            List<Domain.Entiteit.Persoon> personen = entiteitManager.GetAllPeople().ToList();
            WebUI.Models.GrafiekViewModel model = new GrafiekViewModel()
            {
                Personen = entiteitManager.GetAllPeople(),
                Organisaties = entiteitManager.GetAllOrganisaties(),
                Themas = entiteitManager.GetThemas().ToList()
            };
            return View(model);
        }

        //
        // GET: /Manage/AddPhoneNumber
        public virtual ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public virtual async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // Send an SMS through the SMS provider to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
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

        //GET: /Manage/ManageAccount
        public virtual ActionResult ManageAccount()
        {
            Account acc = new Account();
            AccountManager acm = new AccountManager();
            acc = acm.getAccount(User.Identity.GetUserId());
            ViewBag.Firstname = acc.Voornaam;
            ViewBag.Lastname = acc.Achternaam;
            ViewBag.Birthdate = acc.GeboorteDatum.ToString("yyyy-MM-dd"); ;
            ViewBag.Email = acc.Email;

            return View();
        }

        public ActionResult UpdateProfile()
        {
            AccountManager acm = new AccountManager();
            Domain.Account.Account model = acm.getAccount(User.Identity.GetUserId());
            return View(model);
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