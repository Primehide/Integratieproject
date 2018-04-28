using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using WebUI.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebUI
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext<ApplicationUser>.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            app.UseFacebookAuthentication(
               appId: "649587395374720",
               appSecret: "3d06a55333019ccc93cd1c41c2803978");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "347863212605-3nqdgoa8lb8o3u635mhtrn1qfvakebq9.apps.googleusercontent.com",
                ClientSecret = "ie7cI7TBgnVFQqx2nlppAbMo"
            });

            createRoles();
            createSuperAdmin();
        }

        private void createRoles()
        {
            var roleMan = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext<ApplicationUser>()));
            if (!roleMan.RoleExists("Admin"))
            {
                var AdminRol = new IdentityRole();
                AdminRol.Name = "Admin";
                roleMan.Create(AdminRol);
            }
            if (!roleMan.RoleExists("SuperAdmin"))
            {
                var SuperAdminRol = new IdentityRole();
                SuperAdminRol.Name = "SuperAdmin";
                roleMan.Create(SuperAdminRol);
            }
        }

        private void createSuperAdmin()
        {
            var user = new ApplicationUser { UserName = "admin@admin.com" /* + PlatformController.currentPlatform */, Email = "admin@admin.com", TenantId = 0, EmailConfirmed = true };
            ApplicationDbContext<ApplicationUser> context = new ApplicationDbContext<ApplicationUser>();
            ApplicationUserStore<ApplicationUser> userStore = new ApplicationUserStore<ApplicationUser>(context);

            ApplicationUserManager aUM = new ApplicationUserManager(userStore);

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleMngr = new RoleManager<IdentityRole>(roleStore);

            List<IdentityRole> roles = roleMngr.Roles.ToList();
            List<string> roleStrings = new List<string>();
            foreach (IdentityRole ir in roles)
            {
                roleStrings.Add(ir.Name.ToString());
            }

            var result = aUM.CreateAsync(user, "admin");
            aUM.AddToRoleAsync(user.Id.ToString(), "Admin");
            aUM.AddToRoleAsync(user.Id.ToString(), "SuperAdmin");
            //var Iresult = aUM.AddToRolesAsync(user.Id,roleStrings.ToArray());
        }
    }
}