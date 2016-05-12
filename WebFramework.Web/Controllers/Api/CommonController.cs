using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers.Api
{
    public class CommonController: ApiController
    {
        [Route("api/common/timezone")]
        public HttpResponseMessage GetClientTimeZone()
        {
            string timezone = Web.Infrastructure.Util.GetClientTimeZone();
            if (string.IsNullOrEmpty(timezone))
                return null;
            return new HttpResponseMessage()
            {
                Content = new StringContent(timezone)
            };
        }
    }
}