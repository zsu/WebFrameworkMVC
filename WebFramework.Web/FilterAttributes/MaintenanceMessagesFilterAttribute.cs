﻿/// Author: Zhicheng Su
using System;
using System.Web.Mvc;
using App.Common.SessionMessage;
using System.Web;
using App.Common.InversionOfControl;
using Service;
using System.Web.Routing;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using Web.Infrastructure.Extensions;

namespace Web.FilterAttributes
{
    /// <summary>
    /// If we're dealing with ajax requests, any message that is in the view data goes to
    /// the http header.
    /// </summary>
    public class MvcMaintenanceMessagesFilterAttribute : ActionFilterAttribute
    {
        public bool Disabled { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!Disabled)
            {
                var request = filterContext.HttpContext.Request;
                var response = filterContext.HttpContext.Response;
                var settingService = IoC.GetService<ISettingService>();
                var startTime = settingService.GetSettingByKey<DateTime>(Constants.SETTING_KEYS_MAINTENANCE_START);
                var endTime = settingService.GetSettingByKey<DateTime>(Constants.SETTING_KEYS_MAINTENANCE_END);
                var maintenanceMessage = settingService.GetSettingByKey<string>(Constants.SETTING_KEYS_MAINTENANCE_MESSAGE, "The site is under maintenance.");
                var warningLead = settingService.GetSettingByKey<double>(Constants.SETTING_KEYS_MAINTENANCE_WARNING_LEAD, 24 * 60 * 60);
                var maintenanceWarningMessage = settingService.GetSettingByKey<string>(Constants.SETTING_KEYS_MAINTENANCE_WARNING_MESSAGE, string.Format("The site is going to be down for maintenance at {0}.", startTime.ToClientTime()));
                maintenanceWarningMessage = string.Format(maintenanceWarningMessage, startTime.ToClientTime());
                var user = filterContext.HttpContext.User;
                bool canBypass = user != null && user.Identity.IsAuthenticated && user.IsInAnyRole(new List<string> { Constants.ROLE_ADMIN, Constants.PERMISSION_SMOKETEST });
                if (!canBypass && startTime != default(DateTime) && DateTime.UtcNow >= startTime)
                {
                    if (endTime == default(DateTime) || DateTime.UtcNow <= endTime)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "Controller", "Maintenance" }, { "Action", "Index" }, { "Area", "" } });
                    }
                }
                else if (startTime != default(DateTime) && startTime > DateTime.UtcNow)
                {
                    var difference = (startTime - DateTime.UtcNow);
                    if (difference.TotalSeconds < warningLead)
                    {
                        SessionMessageManager.SetMessage(MessageType.Warning, MessageBehaviors.StatusBar, maintenanceWarningMessage, Constants.SETTING_KEYS_MAINTENANCE_WARNING_MESSAGE);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
    public class MaintenanceMessagesFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public bool Disabled { get; set; }
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext filterContext)
        {
            if (!Disabled)
            {
                var request = filterContext.Request;
                var response = filterContext.Response;
                var settingService = IoC.GetService<ISettingService>();
                var startTime = settingService.GetSettingByKey<DateTime>(Constants.SETTING_KEYS_MAINTENANCE_START);
                var endTime = settingService.GetSettingByKey<DateTime>(Constants.SETTING_KEYS_MAINTENANCE_END);
                var maintenanceMessage = settingService.GetSettingByKey<string>(Constants.SETTING_KEYS_MAINTENANCE_MESSAGE, "The site is under maintenance.");
                var warningLead = settingService.GetSettingByKey<double>(Constants.SETTING_KEYS_MAINTENANCE_WARNING_LEAD,24*60*60);
                var maintenanceWarningMessage = settingService.GetSettingByKey<string>(Constants.SETTING_KEYS_MAINTENANCE_WARNING_MESSAGE, string.Format("The site is going to be down for maintenance at {0}.",startTime.ToClientTime()));
                var user = HttpContext.Current.User;
                bool canBypass = user != null && user.Identity.IsAuthenticated && user.IsInAnyRole(new List<string> { Constants.ROLE_ADMIN, Constants.PERMISSION_SMOKETEST });
                if (!canBypass && startTime != default(DateTime) && DateTime.UtcNow >= startTime)
                {
                    if (endTime == default(DateTime) || DateTime.UtcNow <= endTime)
                    {
                        filterContext.Response = new HttpResponseMessage(HttpStatusCode.OK); //response doesn't work
                        string fullyQualifiedUrl = string.Format("{0}/Maintenance", request.RequestUri.GetLeftPart(UriPartial.Authority));
                        //response.Headers.Location = new Uri(fullyQualifiedUrl);
                        filterContext.Response.Headers.Add("FORCE_REDIRECT", fullyQualifiedUrl);
                        return;
                    }
                }
                if (startTime != default(DateTime) && startTime>DateTime.UtcNow)
                {
                    var difference = (startTime - DateTime.UtcNow);
                    if (difference.TotalSeconds < warningLead)
                    {
                        SessionMessageManager.SetMessage(MessageType.Warning, MessageBehaviors.StatusBar, maintenanceWarningMessage, Constants.SETTING_KEYS_MAINTENANCE_WARNING_MESSAGE);
                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}