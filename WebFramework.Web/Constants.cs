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
        public const string SETTING_KEYS_SCHEDULEDTASK_LOGS_EXPIRATION = "scheduledtask.logs.expiration";
        public const string SETTING_KEYS_SCHEDULEDTASK_ACTIVITY_LOGS_EXPIRATION = "scheduledtask.activitylogs.expiration";
        public const string SETTING_KEYS_SCHEDULEDTASK_AUTHENTICATION_AUDIT_EXPIRATION = "scheduledtasks.authenticationaudits.expiration";
        public const string SETTING_KEYS_MAINTENANCE_START = "maintenance.start";
        public const string SETTING_KEYS_MAINTENANCE_END = "maintenance.end";
        public const string SETTING_KEYS_MAINTENANCE_MESSAGE = "maintenance.message";
        public const string SETTING_KEYS_MAINTENANCE_WARNING_LEAD = "maintenance.warninglead";
        public const string SETTING_KEYS_MAINTENANCE_WARNING_MESSAGE = "maintenance.warningmessage";
        #endregion Setting Keys
    }
}