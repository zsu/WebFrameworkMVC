using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;
using App.Common.InversionOfControl;
using App.Mvc.JqGrid;
using System.Text;
using App.Common.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Web.Models;
using System.IO;

namespace Web.Controllers.Api
{
    [Authorize(Roles = Constants.ROLE_ADMIN)]
    public class QuartzTaskController : ApiController
    {
        public QuartzTaskController()
        {
        }
        // GET api/quartztask
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            int totalRecords;
            var dataList = GetQuery(searchModel, out totalRecords);
            var totalPages = (int)Math.Ceiling((float)totalRecords / searchModel.rows);

            var grid = new JqGridModel
            {
                total = totalPages,
                page = searchModel.page,
                records = totalRecords,
                rows = dataList.Select(x => new
                {
                    id = x.TriggerGroup + "," + x.TriggerName,
                    cell = new object[] { x.TriggerGroup + "," + x.TriggerName, x.TriggerGroup, x.TriggerName, x.JobGroup, x.JobName, x.Description, x.CronExpression, x.TimeZone, x.State, x.PreviousTimeUtc, x.StartTimeUtc, x.EndTimeUtc, x.Parameters }
                }).ToArray()
            };

            return grid;
        }
        [Route("api/quartztask/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcel([FromUri]JqGridSearchModel searchModel)
        {
            string filePath = null;
            HttpResponseMessage result = null;
            int totalRecords;
            try
            {
                var dataList = GetQuery(searchModel, out totalRecords);
                filePath = Web.Infrastructure.ExporterManager.Export("QuartzTasks", Web.Infrastructure.ExporterType.CSV, dataList.ToList(), "");
            }
            catch (Exception ex)
            {
                return Web.Infrastructure.Util.DisplayExportError(ex);
            }
            if (!File.Exists(filePath))
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(filePath);
                result.Content.Headers.ContentLength = new FileInfo(filePath).Length;
            }
            return result;
        }
        // GET api/quartztask/5
        public IHttpActionResult Get(Guid id)
        {
            //var item = _service.Query().FirstOrDefault((p) => p.Id == id);
            //if (item == null)
            //{
            //    return NotFound();
            //}
            //return Ok(item);
            return Ok();
        }

        // POST api/quartztask
        public IHttpActionResult Post([FromBody] QuartzTriggerModel item)
        {
            StringBuilder message = new StringBuilder();
            if (item == null)
                return BadRequest("Trigger cannot be empty.");
            if (string.IsNullOrEmpty(item.TriggerGroup))
                return BadRequest("Trigger group cannot be empty.");
            if (string.IsNullOrEmpty(item.TriggerName))
                return BadRequest("Trigger name cannot be empty.");
            if (string.IsNullOrEmpty(item.JobGroup))
                return BadRequest("Job group cannot be empty.");
            if (string.IsNullOrEmpty(item.JobName))
                return BadRequest("Job name cannot be empty.");
            if (string.IsNullOrEmpty(item.CronExpression))
                return BadRequest("Cron expression cannot be empty.");
            item.TriggerGroup = item.TriggerGroup.Trim();
            item.TriggerName = item.TriggerName.Trim();
            if (item.StartTimeUtc == DateTime.MinValue)
                item.StartTimeUtc = DateTime.UtcNow;
            IScheduler scheduler = GetScheduler();
            ITrigger oldTrigger = scheduler.GetTrigger(new TriggerKey(item.TriggerName, item.TriggerGroup));
            if (oldTrigger != null)
                return BadRequest(string.Format("Trigger {0} already exists.", item.TriggerName));
            IDictionary<string, object> dict = null;
            JobDataMap dataMap = null;
            if (!string.IsNullOrEmpty(item.Parameters))
            {
                dict = item.Parameters.Split(',')
              .Select(s => s.Split(new string[] { ":=" }, StringSplitOptions.None))
              .ToDictionary(a => a[0].Trim(), a => !string.IsNullOrEmpty(a[1]) ? (object)a[1].Trim() : null);
                dataMap = new JobDataMap(dict);
            }
            ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(item.TriggerName, item.TriggerGroup)
            .WithCronSchedule(item.CronExpression, x => x.InTimeZone(TimeZoneInfo.Utc))
            .ForJob(item.JobName, item.JobGroup)
            .UsingJobData(dataMap)
            .WithDescription(item.Description)
            .StartAt(item.StartTimeUtc)
            .EndAt(item.EndTimeUtc)
            .Build();
            scheduler.ScheduleJob(trigger);
            message.AppendFormat("Trigger {0}  is saved successflly.", item.TriggerName);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = item.TriggerGroup + "/" + item.TriggerName });
        }

        // PUT api/quartztask/5
        public IHttpActionResult Put(string id, [FromBody] QuartzTriggerModel item)
        {
            StringBuilder message = new StringBuilder();
            if (string.IsNullOrEmpty(id))
                return BadRequest("Item id cannot be empty.");
            if (item == null)
            {
                return BadRequest("Item cannot be empty.");
            }
            if (string.IsNullOrEmpty(item.TriggerName))
            {
                return BadRequest("Item name cannot be empty.");
            }
            if (string.IsNullOrEmpty(item.TriggerGroup))
            {
                return BadRequest("Item group cannot be empty.");
            }
            item.TriggerGroup = item.TriggerGroup.Trim();
            item.TriggerName = item.TriggerName.Trim();
            IScheduler scheduler = GetScheduler();
            ITrigger oldTrigger = scheduler.GetTrigger(new TriggerKey(item.TriggerName, item.TriggerGroup));
            TriggerBuilder tb = oldTrigger.GetTriggerBuilder();
            IDictionary<string, object> dict = null;
            JobDataMap dataMap = null;
            if (!string.IsNullOrEmpty(item.Parameters))
            {
                dict = item.Parameters.Split(',')
              .Select(s => s.Split(new string[] { ":=" }, StringSplitOptions.None))
              .ToDictionary(a => a[0].Trim(), a => !string.IsNullOrEmpty(a[1]) ? (object)a[1].Trim() : null);
                dataMap = new JobDataMap(dict);
            }
            ITrigger newTrigger = TriggerBuilder.Create()
            .WithIdentity(item.TriggerName, item.TriggerGroup)
            .WithCronSchedule(item.CronExpression, x => x.InTimeZone(TimeZoneInfo.Utc))
            .ForJob(item.JobName, item.JobGroup)
            .UsingJobData(dataMap)
            .WithDescription(item.Description)
            .StartAt(item.StartTimeUtc)
            .EndAt(item.EndTimeUtc)
            .Build();
            //ITrigger newTrigger = tb.UsingJobData(dataMap).WithCronSchedule(item.CronExpression).StartAt(item.StartTimeUtc).EndAt(item.EndTimeUtc).WithDescription(item.Description).Build();
            scheduler.RescheduleJob(oldTrigger.Key, newTrigger);

            message.AppendFormat("Item {0}  is saved successflly.", item.TriggerName);
            return Json<object>(new { Success = true, Message = message.ToString(), RowId = id });
        }

        // DELETE api/quartztask/5
        public IHttpActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Trigger id cannot be empty.");
            string[] key = id.Split(',');
            if (key == null || key.Length != 2)
                return BadRequest(string.Format("Invalid trigger key {0}.", id));
            IScheduler scheduler = GetScheduler();
            if (scheduler.UnscheduleJob(new TriggerKey(key[1], key[0])))
                return Ok();
            else
                return NotFound();
        }
        private IQueryable<ITrigger> Query()
        {
            //    System.Collections.Specialized.NameValueCollection properties = new System.Collections.Specialized.NameValueCollection();
            //    properties["quartz.scheduler.instanceName"] = "ServerScheduler";

            //    // set thread pool info
            //    properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            //    properties["quartz.threadPool.threadCount"] = "10";
            //    properties["quartz.threadPool.threadPriority"] = "Normal";
            //    properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            //    properties["quartz.jobStore.dataSource"] = "default";
            //    properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
            //    properties["quartz.jobStore.lockHandler.type"] = "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz";
            //    properties["quartz.jobStore.tablePrefix"] = "QRTZ_";
            //    properties["quartz.jobStore.useProperties"] = "true";

            //    properties["quartz.dataSource.default.connectionString"] = "Server=localhost;Database=Security;Integrated Security=true;";
            //    properties["quartz.dataSource.default.provider"] = "SqlServer-20";

            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler scheduler = schedFact.GetScheduler();

            var triggers = /*from scheduler in schedFact.AllSchedulers*/
                           from triggerKey in scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.AnyGroup())
                           select scheduler.GetTrigger(triggerKey);
            return triggers.AsQueryable<ITrigger>();
        }
        private IQueryable<QuartzTriggerModel> GetQuery(JqGridSearchModel searchModel, out int totalRecords)
        {
            var query = Query().Select(x => new QuartzTriggerModel
            {
                TriggerGroup = x.Key.Group,
                TriggerName = x.Key.Name,
                JobGroup = x.JobKey.Group,
                JobName = x.JobKey.Name,
                Description = x.Description,
                CronExpression = ((ICronTrigger)x).CronExpressionString,
                TimeZone = ((ICronTrigger)x).TimeZone.DisplayName,
                State = GetScheduler().GetTriggerState(x.Key).ToString(),
                PreviousTimeUtc = x.GetPreviousFireTimeUtc().HasValue ? x.GetPreviousFireTimeUtc().Value.DateTime : (DateTime?)null,
                StartTimeUtc = x.StartTimeUtc.DateTime,
                EndTimeUtc = x.EndTimeUtc.HasValue ? x.EndTimeUtc.Value.DateTime : (DateTime?)null,
                Parameters = string.Join(",\n", x.JobDataMap.Select(kv => kv.Key.ToString() + ":=" + kv.Value.ToString()).ToArray())
            }); ;
            if (Constants.SHOULD_FILTER_BY_APP)
                query = query.Where(x => x.TriggerGroup == App.Common.Util.ApplicationConfiguration.AppAcronym);
            searchModel.rows = 0;
            var data = Web.Infrastructure.Util.GetGridData<QuartzTriggerModel>(searchModel, query);
            totalRecords = data.TotalNumber;
            return data.Items;
        }
        private IScheduler GetScheduler()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            IScheduler scheduler = schedFact.GetScheduler();
            return scheduler;
        }

    }
}
