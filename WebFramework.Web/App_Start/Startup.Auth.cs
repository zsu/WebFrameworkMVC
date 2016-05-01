using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using App.Common;
using App.Common.InversionOfControl;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using BrockAllen.MembershipReboot.Nh.Service;
using BrockAllen.MembershipReboot.Owin;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Service;


namespace Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            app.Use((context, next) =>
            {
                //IoC.RegisterInstance<IOwinContext>(context,LifetimeType.PerRequest);
                FirstRequestInitialization.Initialize();
                return next();
            });
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(string.IsNullOrEmpty(ConfigurationManager.AppSettings["AuthCookieExpireTimeSpanInMinutes"]) ? 30 : Int32.Parse(ConfigurationManager.AppSettings["AuthCookieExpireTimeSpanInMinutes"])),
                LoginPath = new PathString("/UserAccount/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            ConfigureMembershipReboot(app);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();
        }
        private static void ConfigureMembershipReboot(IAppBuilder app)
        {
            var cookieOptions = new CookieAuthenticationOptions
            {
                AuthenticationType = MembershipRebootOwinConstants.AuthenticationType,//DefaultAuthenticationTypes.ApplicationCookie,
                ExpireTimeSpan = TimeSpan.FromMinutes(string.IsNullOrEmpty(ConfigurationManager.AppSettings["AuthCookieExpireTimeSpanInMinutes"]) ? 30 : Int32.Parse(ConfigurationManager.AppSettings["AuthCookieExpireTimeSpanInMinutes"])),
                LoginPath = new PathString("/UserAccount/Login")
            };
            RegisterServices(app, cookieOptions.AuthenticationType);
            app.UseMembershipReboot(cookieOptions);
        }
        private static void RegisterServices(IAppBuilder app, string authType)
        {
            var container = IoC.GetContainer<IWindsorContainer>();
            container.Register(Component.For<IOwinContext>().UsingFactoryMethod(() => HttpContext.Current.GetOwinContext()).LifeStyle.Transient);
            var appinfo = new OwinApplicationInformation(app, Util.ApplicationConfiguration.AppAcronym, Util.ApplicationConfiguration.SupportOrganization,
            "UserAccount/Login",
            "UserAccount/ChangeEmail/Confirm/",
            "UserAccount/Register/Cancel/",
            "UserAccount/PasswordReset/Confirm/");
            container.Register(Component.For<ApplicationInformation>().Instance(appinfo).LifeStyle.Singleton);
            container.Register(Component.For<AuthenticationService<NhUserAccount>>().UsingFactoryMethod(ctx => 
            {
                return new OwinAuthenticationService<NhUserAccount>(authType, IoC.GetService<NhUserAccountService<NhUserAccount>>(), IoC.GetService<IOwinContext>().Environment, new Web.Infrastructure.RoleClaimsAuthenticationManager());
            }).LifestylePerWebRequest());
            

            container.Register(Component.For<UserAccountService<NhUserAccount>>().OnCreate(ctx =>
            {
                var debugging = false;
#if DEBUG
                debugging = true;
#endif
                ctx.ConfigureTwoFactorAuthenticationCookies(IoC.GetService<IOwinContext>().Environment, debugging); 
            }).LifestylePerWebRequest());
            container.Register(Component.For<NhUserAccountService<NhUserAccount>>().OnCreate(ctx =>
            {
                var debugging = false;
#if DEBUG
                debugging = true;
#endif
                ctx.ConfigureTwoFactorAuthenticationCookies(IoC.GetService<IOwinContext>().Environment, debugging);
            }).LifestylePerWebRequest()); 

            var config = SecurityConfig.Config(app);//Depends on IMessageTemplateService
            container.Register(Component.For<MembershipRebootConfiguration<NhUserAccount>>().Instance(config));
        }
    }
    class FirstRequestInitialization
    {
        private static bool _initialized = false;
        private static Object _syncRoot = new Object();
        // Initialize only on the first request

        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            lock (_syncRoot)
            {
                if (_initialized)
                {
                    return;
                }
                // Perform first-request initialization here ...
                SeedDatabase();
                _initialized = true;
            }
        }
        private static void SeedDatabase()
        {
            string userEmail = ConfigurationManager.AppSettings[Constants.ADMIN_EMAIL];
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                IUserService userService = IoC.GetService<IUserService>();
                string username = userEmail.Substring(0, userEmail.IndexOf('@') > 0 ? userEmail.IndexOf('@') : userEmail.Length);
                //string username = userEmail;
                var account = userService.FindBy(x => x.Username == username || x.Email == userEmail);
                if (account == null)
                {
                    NhUserAccount user = new NhUserAccount() { Username = username, Email = userEmail, HashedPassword = "Abc123$", FirstName = "Admin", IsLoginAllowed = true, IsAccountVerified = true };
                    account = userService.CreateAccountWithTempPassword(user);
                }

                IRoleService roleServie = IoC.GetService<IRoleService>();
                Role adminRole = new Role { Name = Constants.ROLE_ADMIN, Description = "System Administrator" };
                if (!roleServie.RoleExists(adminRole.Name))
                {
                    roleServie.CreateRole(adminRole);
                }
                if (!roleServie.IsUserInRole(account.ID, Constants.ROLE_ADMIN))
                    roleServie.AddUsersToRoles(new List<Guid>() { account.ID }, new List<string>() { adminRole.Name });
            }
        }
    }
}