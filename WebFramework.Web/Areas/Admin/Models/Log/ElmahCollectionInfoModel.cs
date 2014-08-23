using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models
{
    public class ElmahCollectionInfoModel
    {
        public NameValueCollection Data { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
    }
}