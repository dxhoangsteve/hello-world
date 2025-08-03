using BaseSource.Data.EF;
using BaseSource.Shared.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BaseSource.BackendApi.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class FilterIpAttribute : Attribute, IAsyncActionFilter
    {
        #region allowed
        /// <summary>
        /// Comma seperated string of allowable IPs. Example "10.2.5.41,192.168.0.22"
        /// </summary>
        /// <value></value>
        public string AllowedSingleIPs { get; set; }

        /// <summary>
        /// Gets or sets the configuration key for allowed single IPs
        /// </summary>
        /// <value>The configuration key single I ps.</value>
        public string ConfigurationKeyAllowedSingleIPs { get; set; }

        /// <summary>
        /// List of allowed IPs
        /// </summary>
        private List<IPAddress> allowedIPListToCheck = new List<IPAddress>();
        #endregion

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var _configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            // Populate the IPList with the Single IPs
            if (!string.IsNullOrEmpty(AllowedSingleIPs))
            {
                SplitAndAddSingleIPs(AllowedSingleIPs);
            }

            // Check if there are more settings from the configuration
            if (!string.IsNullOrEmpty(ConfigurationKeyAllowedSingleIPs))
            {
                SplitAndAddSingleIPs(_configuration.GetValue<string>(ConfigurationKeyAllowedSingleIPs));
            }

            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            var badIp = true;

            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            foreach (var ip in allowedIPListToCheck)
            {
                if (ip.Equals(remoteIp))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }

            await next();
        }

        private void SplitAndAddSingleIPs(string ips)
        {
            var splitSingleIPs = ips.Split(';');
            foreach (string item in splitSingleIPs)
            {
                allowedIPListToCheck.Add(IPAddress.Parse(item));
            }
        }
    }
}
