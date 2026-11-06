using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace HelpDesk.Web
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "HelpDeskCookie",
                LoginPath = new PathString("/Account/Login"),
                CookieName = "HelpDesk.Auth",
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest,
                ExpireTimeSpan = TimeSpan.FromHours(8),
                SlidingExpiration = true,
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = context =>
                    {
                        var identity = context.Identity;
                        if (identity != null &&
                            identity.IsAuthenticated &&
                            identity.FindFirst(ClaimTypes.NameIdentifier) == null)
                        {
                            context.RejectIdentity();
                        }

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
