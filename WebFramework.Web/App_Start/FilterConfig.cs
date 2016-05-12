using System.Web.Mvc;
using Web.FilterAttributes;

namespace Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new MvcAjaxMessagesFilterAttribute());
            filters.Add(new MvcMaintenanceMessagesFilterAttribute());
        }
        public static void RegisterWebApiGlobalFilters()
        {
            System.Web.Http.GlobalConfiguration.Configuration.Filters.Add(new AjaxMessagesFilterAttribute());
            System.Web.Http.GlobalConfiguration.Configuration.Filters.Add(new MaintenanceMessagesFilterAttribute());
        }
    }
}
