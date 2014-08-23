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
using Web.Models;

namespace Web.Controllers.Api
{
    public class AuthenticationController: ApiController
    {
        UserAccountService<NhUserAccount> _userAccountService;
        AuthenticationService<NhUserAccount> _authenticationService;
        public AuthenticationController(UserAccountService<NhUserAccount> userAccountService,AuthenticationService<NhUserAccount> authenticationService)
        {
            _userAccountService = userAccountService;
            _authenticationService = authenticationService;
        }
        [HttpPost]
        [Route("api/authenticate")]
        public async Task<IHttpActionResult> Authenticate([FromBody]AuthenticationModel authenticationModel)
        {
            if (authenticationModel == null || string.IsNullOrEmpty(authenticationModel.UserName) || string.IsNullOrEmpty(authenticationModel.Password))
                return Unauthorized();
            NhUserAccount account;
            if (_userAccountService.Authenticate(authenticationModel.UserName, authenticationModel.Password, out account))
            {
                _authenticationService.SignIn(account, false);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}