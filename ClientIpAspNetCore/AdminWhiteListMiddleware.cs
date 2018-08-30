using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ClientIpAspNetCore
{
    public class AdminWhiteListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminWhiteListMiddleware> _logger;
        private readonly string _adminWhiteList;

        public AdminWhiteListMiddleware(RequestDelegate next, ILogger<AdminWhiteListMiddleware> logger, string adminWhiteList)
        {
            _adminWhiteList = adminWhiteList;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            //if (context.Request.Method != "GET")
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                _logger.LogInformation($"Request from Remote IP address: {remoteIp}");

                string[] ip = _adminWhiteList.Split(';');

                // Returns InterNetwork for IPv4 or InterNetworkV6 for IPv6.
                // IP6 
                if (remoteIp.AddressFamily.ToString() == ProtocolFamily.InterNetworkV6.ToString())
                {
                    _logger.LogInformation($"IP6");

                    Byte[] bytes = remoteIp.GetAddressBytes();
                    
                }
                // IP4
                else if (!ip.Any(option => option == remoteIp.ToString()))
                {
                    _logger.LogInformation($"Forbidden Request from Remote IP address: {remoteIp}");
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            await _next.Invoke(context);

        }
    }
}
