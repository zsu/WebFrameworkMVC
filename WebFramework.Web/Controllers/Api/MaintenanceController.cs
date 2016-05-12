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
        public IHttpActionResult DeleteActivityLog()
        {
            DeleteActivityLogsTask task = new DeleteActivityLogsTask();
            task.Execute();
            return Ok();
        }
        [Route("api/maintenance/authenticationaudit")]
        public IHttpActionResult DeleteAuthenticationAudit()
        {
            DeleteAuthenticationAuditsTask task = new DeleteAuthenticationAuditsTask();
            task.Execute();
            return Ok();
        }
        [Route("api/maintenance/log")]
        public IHttpActionResult DeleteLog()
        {
            DeleteLogsTask task = new DeleteLogsTask();
            task.Execute();
            return Ok();
        }
    }
}