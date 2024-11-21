using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System.Configuration;
using System;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(YourNamespace.Startup))]
namespace YourNamespace
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/LoginUser/Index")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            var clientId = ConfigurationManager.AppSettings["GoogleAuth:ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["GoogleAuth:ClientSecret"];

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                CallbackPath = new PathString("/signin-google"),
                Provider = new GoogleOAuth2AuthenticationProvider
                {
                    OnAuthenticated = context =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Authenticated: {context.Identity.Name}");
                        return Task.FromResult(0);
                    },
                    OnReturnEndpoint = context =>
                    {
                        System.Diagnostics.Debug.WriteLine($"Return Endpoint: {context.Identity?.Name ?? "No identity"}");
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}