using System.Collections.Generic;

namespace App.Mvc.JqGrid
{
    public class JqGridFilter
    {
        public GroupOperations groupOp { get; set; }
        public List<JqGridRule> rules { get; set; }
        public List<JqGridFilter> groups { get; set; }
    }
}
