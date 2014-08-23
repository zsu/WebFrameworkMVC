using System.Web.Mvc;

namespace Web.Areas.UserAccount
{
    public class UserAccountAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserAccount";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("oauth2",
                BrockAllen.OAuth2.OAuth2Client.OAuthCallbackUrl,
                new { controller = "LinkedAccount", action = "OAuthCallback" });


            context.MapRoute(
                "UserAccount_default",
                "UserAccount/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
