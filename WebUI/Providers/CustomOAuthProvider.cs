using BL;
using Domain.Platform;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebUI.Models;

namespace WebUI.Providers
{
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string platformId = context.Parameters.Where(f => f.Key == "platId").Select(f => f.Value).SingleOrDefault()[0];
            context.OwinContext.Set<string>("platformId", platformId);
            string deviceId = context.Parameters.Where(f => f.Key == "device").Select(f => f.Value).SingleOrDefault()[0];
            context.OwinContext.Set<string>("device", deviceId);
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public static ApplicationUserManager makeManager(OAuthGrantResourceOwnerCredentialsContext cont, int platId)
        {
            var context = cont.OwinContext.Get<ApplicationDbContext<ApplicationUser>>();
            var userstore = new ApplicationUserStore<ApplicationUser>(context) { TenantId = platId };
            ApplicationUserManager uM = new ApplicationUserManager(userstore);
            return uM;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = "*";

            string platformId = context.OwinContext.Get<string>("platformId");
            string deviceId = context.OwinContext.Get<string>("device");


            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var userManager = makeManager(context, Int32.Parse(platformId));
            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            AccountManager aM = new AccountManager();
            aM.addDeviceId(user.Id, deviceId);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.EmailConfirmed)
            {
                context.SetError("invalid_grant", "User did not confirm email.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");

            var ticket = new AuthenticationTicket(oAuthIdentity, null);

            context.Validated(ticket);

        }
    }
}