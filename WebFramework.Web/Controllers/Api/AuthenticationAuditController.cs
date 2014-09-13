﻿using App.Common.Logging;
using App.Mvc.JqGrid;
using BrockAllen.MembershipReboot.Nh;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;
using Web.Infrastructure.Exceptions;
using WebFramework.Data.Domain;
using App.Common.InversionOfControl;
using Web.Infrastructure;
using System.IO;

namespace Web.Controllers.Api
{
    public class AuthenticationAuditController : ApiController
    {
        private IAuthenticationAuditService _service;
        public AuthenticationAuditController()
        {
            _service = IoC.GetService<IAuthenticationAuditService>();
        }

        [Authorize(Roles = Constants.ROLE_ADMIN)]
        public dynamic GetGridData([FromUri] JqGridSearchModel searchModel)
        {
            var query = _service.Query();
            if (Constants.SHOULD_FILTER_BY_APP)
                query = query.Where(x => x.Application == App.Common.Util.ApplicationConfiguration.AppAcronym);
            var data = Util.GetGridData<AuthenticationAudit>(searchModel, query);
            var dataList = data.Items.ToList(); 
            var grid = new JqGridModel
            {
                total = data.TotalPage,
                page = data.CurrentPage,
                records = data.TotalNumber,
                rows = dataList.Select(x => new { id = x.Id, cell = new object[] { x.Id, x.Application, x.CreatedDate, x.Activity, x.Detail, x.UserName, x.ClientIP } }).ToArray()
            };
            return grid;
        }
        [Route("api/authenticationaudit/exporttoexcel")]
        [HttpGet]
        public dynamic ExportToExcel([FromUri]JqGridSearchModel searchModel)
        {
            var query = _service.Query();
            if (Constants.SHOULD_FILTER_BY_APP)
                query = query.Where(x => x.Application == App.Common.Util.ApplicationConfiguration.AppAcronym);
            searchModel.rows = 0;
            var data = Util.GetGridData<AuthenticationAudit>(searchModel, query);
            var dataList = data.Items.Select(x => new { x.Id, x.Application, x.CreatedDate, x.Activity, x.Detail, x.UserName, x.ClientIP }).ToList();
            string filePath = ExporterManager.Export("authenticationaudit", ExporterType.CSV, dataList.ToList(), "");
            HttpResponseMessage result = null;

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
    }
}
