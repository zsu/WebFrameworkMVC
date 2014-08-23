using System;
using System.Threading;
using Common.Logging;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SLib.Cryptography;
namespace Quartz.Jobs
{
    /// <summary>
    /// A sample job that just prints info on console for demostration purposes.
    /// </summary>
    public class RestfulServiceJob : IJob
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(RestfulServiceJob));

        /// <summary>
        /// Called by the <see cref="IScheduler" /> when a <see cref="ITrigger" />
        /// fires that is associated with the <see cref="IJob" />.
        /// </summary>
        /// <remarks>
        /// The implementation may wish to set a  result object on the 
        /// JobExecutionContext before this method exits.  The result itself
        /// is meaningless to Quartz, but may be informative to 
        /// <see cref="IJobListener" />s or 
        /// <see cref="ITriggerListener" />s that are watching the job's 
        /// execution.
        /// </remarks>
        /// <param name="context">The execution context.</param>
        public void Execute(IJobExecutionContext context)
        {
            logger.Debug("RestfulServiceJob starts.");
            JobDataMap dataMap = context.MergedJobDataMap;  // Note the difference from the previous example

            string baseUrl = dataMap.GetString("baseUrl");
            string authenticationUrl = dataMap.GetString("authenticationUrl");
            string taskUrl = dataMap.GetString("taskUrl");
            string application = dataMap.GetString("application");
            string userName = dataMap.GetString("userName");
            string password = dataMap.GetString("password");
            if(!string.IsNullOrEmpty(password))
            {
                TripleDesProtectedConfigurationProvider cryptographyProvider = new TripleDesProtectedConfigurationProvider();
                password=cryptographyProvider.DecryptString(password);
            }
            CallWebService(baseUrl, authenticationUrl, taskUrl, application, userName, password);
            logger.Debug("RestfulServiceJob run finished.");
        }
        private void CallWebService(string baseUrl, string authenticationUrl, string taskUrl, string application, string userName, string password)
        {
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var httpClient = new HttpClient(handler))
            {
                httpClient.BaseAddress = new Uri(baseUrl);

                httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                var result = httpClient.PostAsJsonAsync(authenticationUrl, new
                {
                    Application=application,
                    UserName = userName,
                    Password = password
                }).Result;
                result = httpClient.DeleteAsync(taskUrl).Result;

                result.EnsureSuccessStatusCode();

                //var userProfile = await result.Content.ReadAsAsync<UserProfile>();

                //if (userProfile == null)
                //    throw new UnauthorizedAccessException();

                //return userProfile;
            }
        }
    }
}