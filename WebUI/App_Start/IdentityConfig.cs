using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SendGrid;
using SendGrid.Helpers.Mail;
using WebUI.Models;
using WebUI.Controllers;
using System.Data.Entity.SqlServer.Utilities;

namespace WebUI
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        private async Task configSendGridasync(IdentityMessage message)
        {
            var apiKey = WebConfigurationManager.AppSettings["SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new SendGrid.Helpers.Mail.EmailAddress(WebConfigurationManager.AppSettings["mailAccount"], "Optimize Prime");
            myMessage.Subject = message.Subject;
            myMessage.PlainTextContent = message.Body;
            myMessage.HtmlContent = message.Body;
            var response = await client.SendEmailAsync(myMessage);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        private static IOwinContext cont;
        private static IdentityFactoryOptions<ApplicationUserManager> opt;


        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            //var manager = new ApplicationUserManager(new ApplicationUserStore<ApplicationUser>(new ApplicationDbContext<ApplicationUser>()) { TenantId = 0 });
            // Configure validation logic for usernames
            this.UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false //makes it possible to link multipe external logins
            };

            // Configure validation logic for passwords
            this.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            this.UserLockoutEnabledByDefault = true;
            this.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            this.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            this.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            this.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            this.EmailService = new EmailService();
            this.SmsService = new SmsService();
            try
            {
                if (opt.DataProtectionProvider != null)
                {
                    var dataProtectionProvider = opt.DataProtectionProvider;

                    this.UserTokenProvider =
                        new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
                }
            } catch (Exception e)
            {
                
            }
        }



        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            cont = context;
            opt = options;
            ApplicationDbContext<ApplicationUser> pp = new ApplicationDbContext<ApplicationUser>();
            ApplicationUserStore<ApplicationUser> userStore = new ApplicationUserStore<ApplicationUser>(pp);

            return new ApplicationUserManager(userStore);
        }
    }

    public class ApplicationUserStore<TUser> : UserStore<TUser>
        where TUser : ApplicationUser
    {
        public ApplicationUserStore(DbContext context)
          : base(context)
        {
        }

        public int TenantId { get; set; }

        public override Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            user.TenantId = this.TenantId;
            return base.CreateAsync(user);
        }

        public override Task<TUser> FindByEmailAsync(string email)
        {
            return this.GetUserAggregateAsync(x => x.Email.ToUpper() == email.ToUpper()
                && x.TenantId == this.TenantId);
        }

        public override Task<TUser> FindByNameAsync(string userName)
        {
            return this.GetUserAggregateAsync(u => u.UserName.ToUpper() == userName.ToUpper()
                  && u.TenantId == this.TenantId );
        }
    }
        // Configure the application sign-in manager which is used in this application.
        public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        private static IOwinContext cont;
        private static IdentityFactoryOptions<ApplicationUserManager> opt;

        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        //public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        //{
        //    return new ApplicationSignInManager(new ApplicationUserManager(new ApplicationUserStore<ApplicationUser>(context.Get<ApplicationDbContext())), context.Authentication);
        //}
    }
}
