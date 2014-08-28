using App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web
{
    public static class Constants
    {
        #region AppSettingKeys
        public const string ADMIN_EMAIL="AdminEmail";
        public const string HIBERNATE_CONFIG_KEY = "HibernateConfig";
        public const string HIBERNATE_CONFIG_KEY_Security = "HibernateConfigSecurity";
        public const string HIBERNATE_CONFIG_KEY_Log = "HibernateConfigLog";
        public const string HIBERNATE_CONFIG_KEY_App = "HibernateConfigApp";
        public const string APPSETTING_KEY_SETTING_KEY_START_WITH_APPNAME = "SettingKeyStartWithAppName";
        #endregion AppSettingKeys
        #region ConnectionStringKeys
        public const string SECURITY_DB = App.Common.Util.SecurityDBConnectionStringName;
        public const string LOG_DB = App.Common.Util.LogDBConnectionStringName;
        public const string APP_DB = App.Common.Util.AppDBConnectionStringName;
        #endregion ConnectionStringKeys
        #region Roles
        public const string ROLE_ADMIN="Admin";
        public const string ROLE_USERADMIN = "UserAdmin";
        #endregion Roles
        #region Permissions
        public const string PERMISSION_EDIT_USER = "EditUser";
        public const string PERMISSION_EDIT_ROLE = "EditRole";
        public const string PERMISSION_EXECUTE_JOB = "ExecuteJob";
        public const string PERMISSION_SMOKETEST = "SmokeTest";
        #endregion Permissions
        #region Setting Keys
        public static bool IS_SETTING_KEY_START_WITH_APPNAME = false;
        private static bool success = bool.TryParse(string.IsNullOrWhiteSpace(System.Configuration.ConfigurationManager.AppSettings[APPSETTING_KEY_SETTING_KEY_START_WITH_APPNAME]) ? string.Empty :
        System.Configuration.ConfigurationManager.AppSettings[APPSETTING_KEY_SETTING_KEY_START_WITH_APPNAME], out IS_SETTING_KEY_START_WITH_APPNAME);
        public const string SETTING_KEYS_SCHEDULEDTASK_LOGS_EXPIRATION = "scheduledtask.logs.expiration";
        public const string SETTING_KEYS_SCHEDULEDTASK_ACTIVITY_LOGS_EXPIRATION = "scheduledtask.activitylogs.expiration";
        public const string SETTING_KEYS_SCHEDULEDTASK_AUTHENTICATION_AUDIT_EXPIRATION = "scheduledtasks.authenticationaudits.expiration";
        public static readonly string SETTING_KEYS_MAINTENANCE_START = (IS_SETTING_KEY_START_WITH_APPNAME ? Util.ApplicationConfiguration.AppAcronym + "." : String.Empty) + "maintenance.start";
        public static readonly string SETTING_KEYS_MAINTENANCE_END = (IS_SETTING_KEY_START_WITH_APPNAME ? Util.ApplicationConfiguration.AppAcronym + "." : String.Empty) + "maintenance.end";
        public static readonly string SETTING_KEYS_MAINTENANCE_MESSAGE = (IS_SETTING_KEY_START_WITH_APPNAME ? Util.ApplicationConfiguration.AppAcronym + "." : String.Empty) + "maintenance.message";
        public static readonly string SETTING_KEYS_MAINTENANCE_WARNING_LEAD = (IS_SETTING_KEY_START_WITH_APPNAME ? Util.ApplicationConfiguration.AppAcronym + "." : String.Empty) + "maintenance.warninglead";
        public static readonly string SETTING_KEYS_MAINTENANCE_WARNING_MESSAGE = (IS_SETTING_KEY_START_WITH_APPNAME ? Util.ApplicationConfiguration.AppAcronym + "." : String.Empty) + "maintenance.warningmessage";
        #endregion Setting Keys
    }
}