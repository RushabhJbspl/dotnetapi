using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Worldex.Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Worldex.Web.Filters
{
    #region snippet_ClassOnly
    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminSafeListMiddleware> _logger;
        private readonly string _adminSafeList;

        public AdminSafeListMiddleware(
            RequestDelegate next,
            ILogger<AdminSafeListMiddleware> logger,
            string adminSafeList)
        {
            _adminSafeList = adminSafeList;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            Console.WriteLine("Current Log: " + "AdminSafeListMiddleware Invoke: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (context.Request.Method != "GET")
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                //_logger.LogDebug($"Request from Remote IP address: {remoteIp}");

                string[] ip = _adminSafeList.Split(';');

                var bytes = remoteIp.GetAddressBytes();
                var badIp = true;
                foreach (var address in ip)
                {
                    var testIp = IPAddress.Parse(address);
                    if (testIp.GetAddressBytes().SequenceEqual(bytes))
                    {
                        badIp = false;
                        break;
                    }
                }

                if (badIp)
                {
                    _logger.LogInformation(
                        $"Forbidden Request from Remote IP address: {remoteIp}");
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    HelperForLog.WriteLogIntoFile("IP Validator", "Request from Remote IP address is ", Convert.ToString(remoteIp));
                    return;
                }
            }
            Console.WriteLine("Current Log: " + "AdminSafeListMiddleware Next: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            await _next.Invoke(context);
            Console.WriteLine("Current Log: " + "AdminSafeListMiddleware Complete: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        }
    }
    #endregion
}
