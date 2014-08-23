using App.Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.FilterAttributes
{
    public class DeleteFileFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.Result is FilePathResult)
            {
                string fileName = ((FilePathResult)filterContext.Result).FileName;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    filterContext.HttpContext.Response.Flush();
                    filterContext.HttpContext.Response.End();
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception exception)
                    {
                        Logger.Log(LogLevel.Error, string.Format("Cannot delete file {0}", fileName), exception);
                    }
                }
            }
        }
    }
}