using System;
using System.Threading.Tasks;
using Gravity.Diagnostics;
using Gravity.Manager.Service;
using Gravity.Manager.Web.Application;
using Gravity.Manager.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gravity.Manager.Web.Controllers.API
{
    [Route("api/discovery")]
    public class DiscoveryController : Controller
    {
        private readonly IDiscoveryService _discoveryService;
        private readonly ILogger _logger;

        public DiscoveryController(IDiscoveryService discoveryService, ILogger logger)
        {
            _discoveryService = discoveryService ?? throw new ArgumentNullException(nameof(discoveryService));
            _logger = (logger ?? throw new ArgumentNullException(nameof(logger))).GetLogger(typeof(DiscoveryController));
        }

        /// <summary>
        /// Posts a discovery session: a set of dependency findigs.
        /// <para />
        /// JSON request example:
        /// { "DependencyFindings": [
        /// {"SourceIp":"1.2.3.4","TargetIp":"5.6.7.8","FileName":"filename.txt","Text":"myText"},
        /// {"SourceIp":"1.2.3.4","TargetIp":"5.6.7.8","FileName":"filename.txt","Text":"myText"}],
        /// "DiscoveryReports": [
        /// "AwsInstanceIpAddress": "1.2.3.4",
        /// "ReportData" : { "Network": ["foo"], "Services": {"bar":"baz"}}}
        /// ]
        /// </summary>
        [HttpPost("{awsAccountName}")]
        public async Task<IActionResult> Post(string awsAccountName, [FromBody] DiscoverySessionResultViewModel session)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warn($"Invalid {nameof(DiscoveryController)}.{nameof(Post)} request: {ModelState}");
                return ModelState.ToValidationErrorResult(); 
            }

            var ses = session.ToDiscoverySessionInfo();
            var awsAcc = await _discoveryService.GetOrCreateAwsAccountAsync(awsAccountName);
            ses.AwsAccountId = awsAcc.Id;

            await _discoveryService.InsertDiscoverySessionAsync(ses);

            return StatusCode(StatusCodes.Status200OK);
        }
    }
}