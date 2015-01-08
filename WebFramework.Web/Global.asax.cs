using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Hierarchical;
using System.Configuration;
using App.Common.InversionOfControl;
using System.IO;
using Castle.Windsor;
using App.Common.Tasks;
using App.Common.Logging;
using System.Reflection;
using System.Text;
using App.Common;
using Web.Controllers.Controllers;
using System.Threading;
using Service;
using App.Common.Configuration;
using App.Infrastructure.NHibernate;
using BrockAllen.MembershipReboot.Nh;
using App.Infrastructure.Castle.Mvc;
using Elmah.Contrib.WebApi;
using System.Text.RegularExpressions;


namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private log4net.ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            IoC.InitializeWith(new DependencyResolverFactory());
            var container = IoCConfig.ConfigureContainer();

            log4net.GlobalContext.Properties["AppName"] = Util.ApplicationConfiguration.AppAcronym;
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Util.GetFullPath(Util.LogConfigFilePath)));
            Web.Infrastructure.Util.ChangeLogLevels();
            //GlobalConfiguration.Configuration.DependencyResolver = (System.Web.Http.Dependencies.IDependencyResolver)IoC.GetResolver<ICustomDependencyResolver>();
            GlobalConfiguration.Configuration.Services.Replace(typeof(System.Web.Http.Dispatcher.IHttpControllerActivator), new WindsorCompositionRoot(container));

            GlobalConfiguration.Configuration.Filters.Add(new ElmahHandleErrorApiAttribute());
            var controllerFactory = new WindsorControllerFactory(IoC.GetContainer<IWindsorContainer>().Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);


            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            FilterConfig.RegisterWebApiGlobalFilters();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Logger.Log(LogLevel.Info, LogType.Application.ToString(), "Application_Start event fired.");

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Server.ClearError();
            if (HttpContext.Current.Request.Url.PathAndQuery.Contains("favicon.ico")) //Ignore favicon.ico not found error
                return;
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }
            //Exception could happens in login page, at which time the returnurl is error.aspx
            if (HttpContext.Current.Request.Url.PathAndQuery.Contains("Error.aspx") || HttpContext.Current.Request.Url.PathAndQuery.Contains("/Error") || currentController == "Error")
            { WriteUnlogErrorPage(exception); }
            else
            {
                HandleError(exception, sender);
            }
            //if (HttpContext.Current.Request.Url.PathAndQuery.Contains("favicon.ico")) //Ignore favicon.ico not found error
            //    return;
            //Exception exception = Server.GetLastError();
            //Server.ClearError();
            //Logger.Log(LogLevel.Error, exception);
        }
        protected void Application_BeginRequest(Object source, EventArgs e)
        {
            if ((VirtualPathUtility.ToAbsolute("~/") != Request.ApplicationPath) && (Request.ApplicationPath.ToLowerInvariant() == Request.Path.ToLowerInvariant()))
            {
                var redirectPath = VirtualPathUtility.AppendTrailingSlash(Request.Path);

                Response.RedirectPermanent(redirectPath);

                return;
            } 
            HttpApplication app = (HttpApplication)source;
            HttpContext context = app.Context;
            // Attempt to peform first request initialization
            FirstRequestInitialization.Initialize(context);
        }
        protected void Application_PostAuthenticateRequest()
        {
            //if (Request.IsAuthenticated)
            //{
            //    IRoleService roleService = IoC.GetService<IRoleService>();
            //    var id = ClaimsPrincipal.Current.Identities.First();
            //    List<Role> roles = roleService.GetRolesForUser(id.GetUserID());
            //    foreach (var role in roles)
            //    {
            //        id.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            //        if(role.Permissions!=null && role.Permissions.Count>0)
            //        foreach (var permission in role.Permissions)
            //            id.AddClaim(new Claim(ClaimTypes.Role, permission.Name));
            //    }
            //}
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            if (HttpContext.Current.Session != null)
                log4net.ThreadContext.Properties["SessionId"] = HttpContext.Current.Session.SessionID;
            //if (Logger.IsLoggingEnabled(LogLevel.Debug))
            //{
            //    StringBuilder logMessage = new StringBuilder();
            //    logMessage.AppendLine("Session Start");
            //    logMessage.AppendLine(string.Format("Session ID: {0}", Session.SessionID));
            //    logMessage.AppendLine(LogBrowserInfo());
            //    Logger.Log(LogLevel.Debug, logMessage.ToString());
            //}
        }
        protected void Application_End(object sender, EventArgs e)
        {
            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine("Application_End event fired.");

            HttpRuntime runtime = (HttpRuntime)typeof(System.Web.HttpRuntime).InvokeMember("_theRuntime",
                                                                               BindingFlags.NonPublic
                                                                               | BindingFlags.Static
                                                                               | BindingFlags.GetField,
                                                                               null,
                                                                               null,
                                                                               null);

            if (runtime != null)
            {
                string shutDownMessage = (string)runtime.GetType().InvokeMember("_shutDownMessage",
                                                                                 BindingFlags.NonPublic
                                                                                 | BindingFlags.Instance
                                                                                 | BindingFlags.GetField,
                                                                                 null,
                                                                                 runtime,
                                                                                 null);
                string shutDownStack = (string)runtime.GetType().InvokeMember("_shutDownStack",
                                                                               BindingFlags.NonPublic
                                                                               | BindingFlags.Instance
                                                                               | BindingFlags.GetField,
                                                                               null,
                                                                               runtime,
                                                                               null);
                logMessage.AppendLine(String.Format("\r\n\r\n_shutDownMessage={0}\r\n\r\n_shutDownStack={1}",
                             shutDownMessage,
                             shutDownStack));
            }
            Logger.Log(LogLevel.Info, LogType.Application.ToString(), logMessage.ToString());

            ////if (!EventLog.SourceExists(".NET Runtime"))
            ////{
            ////    EventLog.CreateEventSource(".NET Runtime", "Application");
            ////}
            ////EventLog log = new EventLog();
            ////log.Source = ".NET Runtime";
            ////log.WriteEntry(String.Format("\r\n\r\n_shutDownMessage={0}\r\n\r\n_shutDownStack={1}",
            ////                             shutDownMessage,
            ////                             shutDownStack),
            ////                             EventLogEntryType.Error);
        }
        #region Private Methods
        private string LogBrowserInfo()
        {
            StringBuilder browserMessage = new StringBuilder();

            try
            {
                browserMessage.AppendLine("**** BEGIN BROWSER INFO ****");
                browserMessage.Append("* User's IP Address: ");
                browserMessage.AppendLine(HttpContext.Current.Request.UserHostAddress);
                browserMessage.Append("* User's DNS: ");
                browserMessage.AppendLine(HttpContext.Current.Request.UserHostName);
                browserMessage.Append("* Client Platform: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.Platform);
                browserMessage.Append("* Browser: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.Type);
                browserMessage.Append("* Browser Ver: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.Version);
                browserMessage.Append("* Client CLR Ver: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.ClrVersion.ToString());
                browserMessage.Append("* ECMA Script Ver: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.EcmaScriptVersion.ToString());
                browserMessage.Append("* MS DOM Ver: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.MSDomVersion.ToString());
                browserMessage.Append("* W3C DOM Ver: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.W3CDomVersion.ToString());
                browserMessage.Append("* Using AOL: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.AOL.ToString());
                browserMessage.Append("* Supports Tables: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.Tables.ToString());
                browserMessage.Append("* Supports Cookies: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.Cookies.ToString());
                browserMessage.Append("* Supports Frames: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.Frames.ToString());
                browserMessage.Append("* Supports Java Applets: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.JavaApplets.ToString());
                browserMessage.Append("* Supports Java Script: ");
                browserMessage.AppendLine((HttpContext.Current.Request.Browser.EcmaScriptVersion.Major >= 1).ToString());
                browserMessage.Append("* Supports ActiveX: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.ActiveXControls.ToString());
                browserMessage.Append("* Browser is Crawler: ");
                browserMessage.AppendLine(HttpContext.Current.Request.Browser.Crawler.ToString());
                browserMessage.AppendLine("**** END BROWSER INFO ****");
            }
            catch
            {
                browserMessage.AppendLine("**** UNABLE TO CAPTURE BROWSER INFO ****");
            }
            return browserMessage.ToString();

        } // LogBrowserInfo
        private void HandleError(System.Exception exception, object sender)
        {
            if (exception is System.Web.HttpUnhandledException &&
                exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            System.Exception newException = exception as System.Exception;

            if (newException == null)
            {
                newException = new System.Exception(exception.Message, exception);
            }
            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            try
            {
                #region Return error message for the following exceptions
                //if (exception is HttpRequestValidationException)
                //{
                //    SessionMessageManager.SetMessage(MessageType.Error, MessageBehaviors.StatusBar, Resources.Std.RequestValidationError);
                //    if (Logger.IsLoggingEnabled(LogLevel.Info))
                //        Logger.Log(LogLevel.Info, exception);
                //    string currentUrl = GetCurrentUrl();
                //    HttpContext.Current.Response.Redirect(currentUrl);
                //    return;
                //}
                ////else if (exception is HttpException && exception.Message.Trim().ToLower() == "maximum request length exceeded.")
                ////{
                ////    SessionMessageManager.SetMessage(MessageType.Error, MessageBehaviors.StatusBar, exception.Message);
                ////    if (_log.IsInfoEnabled)
                ////        _log.Info(exception);
                ////    string sUrl = GetCurrentUrl();
                ////    HttpContext.Current.Response.Redirect(sUrl);
                ////    return;
                ////}
                #endregion
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
                        logMsg.AppendFormat("User ID: {0}{1}", Context.User.Identity.Name, System.Environment.NewLine);
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
                        if (!string.IsNullOrWhiteSpace(currentController))
                            logMsg.AppendFormat("Requested Controller\\Action: {0}\\{1}.{2}", currentController, currentAction, System.Environment.NewLine);
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

                    Logger.Log(LogLevel.Error, logMsg.ToString(), newException);
                }
                else
                {
                    WriteUnlogErrorPage(exception);
                    return;
                }
            }
            catch (Exception exception1)
            {
                WriteUnlogErrorPage(exception1, exception);
                return;
            }
            var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Index";

            if (newException is HttpException)
            {
                var httpEx = newException as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    default:
                        action = "Index";
                        break;
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = newException is HttpException ? ((HttpException)newException).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = action;

            controller.ViewData.Model = new HandleErrorInfo(newException, currentController, currentAction);
            //((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));

            httpContext.Response.Redirect("~/Error");
            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.Redirect("~/Error.aspx");//Errors are logged, display custom error page without disclosing the detail exception info
        }
        private void WriteUnlogErrorPage(params Exception[] exceptionList)
        {
            StringBuilder message = new StringBuilder();
            Server.ClearError();
            HttpContext.Current.Response.Clear();
            message.AppendLine(@"<html><body><div id=""content"">
                <h1>Error</h1>
                <p>
                    The application encountered an error while processing your request. 
                    Please close this browser window and try logging in again.
                    If the problem persists, contact Customer Support for assistance.
                </p>
                <p>Error Message: The following errors are not logged.<br/>");
            message.AppendFormat("Requested file physical path: {0}{1}", HttpContext.Current.Request.PhysicalPath, System.Environment.NewLine);
            foreach (Exception exception in exceptionList)
            {
                Exception exception1 = exception;
                while (exception1 != null)
                {
                    message.Append(exception1.StackTrace);
                    message.Append(": ");
                    message.Append(exception1.Message);
                    message.Append(@"<br/>");
                    message.Append(@"<br/>");
                    exception1 = exception1.InnerException;
                }
            }
            message.Append("</p></div></body></html>");
            HttpContext.Current.Response.Write(message.ToString());
            HttpContext.Current.Response.End();
        }
        private string GetCurrentUrl()
        {
            StringBuilder queryString = new StringBuilder();
            UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host,
                HttpContext.Current.Request.Url.Port, HttpContext.Current.Request.Path);
            foreach (string sQuery in HttpContext.Current.Request.QueryString)
            {
                queryString.Append(sQuery);
                queryString.Append("=");
                queryString.Append(HttpContext.Current.Request.QueryString[sQuery]);
                queryString.Append("&");
            }
            if (queryString.Length > 0)
                queryString.Remove(queryString.Length - 1, 1);
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.ToString();
        }
        #endregion Private Methods
    }
    class FirstRequestInitialization
    {
        private static bool _initialized = false;
        private static Object _syncRoot = new Object();
        // Initialize only on the first request

        public static void Initialize(HttpContext context)
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
                //string username = userEmail.Substring(0, userEmail.IndexOf('@') > 0 ? userEmail.IndexOf('@') : userEmail.Length);
                string username = userEmail;
                var account = userService.FindBy(x => x.Username == username || x.Email == userEmail);
                if (account == null)
                {
                    NhUserAccount user = new NhUserAccount() { Username = username, Email = userEmail, HashedPassword = "Abc123$", FirstName = "Admin",IsLoginAllowed=true,IsAccountVerified=true};
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
