using System.Web;
using System.Web.Mvc;
using App.Common.Logging;
using System.Text;
using App.Common;


namespace Web.FilterAttributes
{
    public class LogExceptionFilterAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            StringBuilder logMsg = new StringBuilder();
            if (Logger.IsLoggingEnabled(LogLevel.Error))   //If there is something wrong in the log4net configuration file,it won't throw exception but all log levels are disabled.
            {
                try
                {
                    //Some exception like file not found happened before the session object is created. 
                    if (HttpContext.Current.Session != null)
                        logMsg.AppendFormat("Session ID: {0}{1}", HttpContext.Current.Session.SessionID, System.Environment.NewLine);
                }
                catch
                {
                }
                try
                {
                    logMsg.AppendFormat("User ID: {0}{1}", HttpContext.Current.User.Identity.Name, System.Environment.NewLine);
                }
                catch
                {
                }
                try
                {
                    logMsg.AppendFormat("An error occurred in {0}.{1}", Util.ApplicationConfiguration.AppFullName, System.Environment.NewLine);
                }
                catch
                {
                }
                try
                {
                    logMsg.AppendFormat("Requested Controller\\Action: {0}\\{1}.{2}", filterContext.Controller, filterContext.RouteData.Values["action"].ToString(), System.Environment.NewLine);
                }
                catch
                {
                } 
                try
                {
                    logMsg.AppendFormat("Requested file physical path: {0}", HttpContext.Current.Request.PhysicalPath);
                }
                catch
                {
                }
                Logger.Log(LogLevel.Error, logMsg.ToString(), filterContext.Exception);
            }
            //Logger.Log(LogLevel.Error,"Error Caught in LogExceptionFilterAttribute: " + filterContext.Controller, filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}