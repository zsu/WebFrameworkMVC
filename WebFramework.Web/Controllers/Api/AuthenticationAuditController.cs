using App.Common.Logging;
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
            var data = Util.GetGridData<AuthenticationAudit>(searchModel, _service.Query());
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
    }
}
