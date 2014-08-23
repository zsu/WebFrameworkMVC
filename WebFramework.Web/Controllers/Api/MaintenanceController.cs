using App.Common.InversionOfControl;
using App.Common.Logging;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Web.Infrastructure.Tasks;

namespace Web.Controllers.Api
{
    [Authorize(Roles = Constants.ROLE_ADMIN + "," + Constants.PERMISSION_EXECUTE_JOB)]
    public class MaintenanceController: ApiController
    {
        public MaintenanceController()
        {
        }
        [Route("api/maintenance/activitylog")]
        public async Task<IHttpActionResult> DeleteActivityLog()
        {
            DeleteActivityLogsTask task = new DeleteActivityLogsTask();
            task.Execute();
            return Ok();
        }
        [Route("api/maintenance/authenticationaudit")]
        public async Task<IHttpActionResult> DeleteAuthenticationAudit()
        {
            DeleteAuthenticationAuditsTask task = new DeleteAuthenticationAuditsTask();
            task.Execute();
            return Ok();
        }
        [Route("api/maintenance/log")]
        public async Task<IHttpActionResult> DeleteLog()
        {
            DeleteLogsTask task = new DeleteLogsTask();
            task.Execute();
            return Ok();
        }
    }
}