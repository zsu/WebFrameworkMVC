using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class QuartzTriggerModel
    {
        public string TriggerGroup {get;set;}
        public string TriggerName { get; set; }
        public string JobGroup { get; set; }
        public string JobName { get; set; }

        public string Description { get; set; }
        public string CronExpression { get; set; }
        public string TimeZone { get; set; }
        public DateTime? PreviousTimeUtc { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime? EndTimeUtc {get;set;}
        public string State { get; set; }
        public string Parameters { get; set; }

    }
}