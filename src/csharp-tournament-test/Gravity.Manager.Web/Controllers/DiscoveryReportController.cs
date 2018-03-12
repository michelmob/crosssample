using System;
using System.Linq;
using System.Threading.Tasks;
using Gravity.Manager.ApplicationService;
using Gravity.Manager.Service;
using Gravity.Manager.Web.Models;
using Gravity.Manager.Web.Models.DiscoveryReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gravity.Manager.Web.Controllers
{
    [Authorize]
    [Route("discovery-reports")]
    public class DiscoveryReportController : Controller
    {
        private readonly IDiscoveryAppService _service;

        public DiscoveryReportController(IDiscoveryAppService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _service.GetDiscoverySessionsWithAccountsAsync();

            var sessionModels = sessions.Select(x => new DiscoverySessionViewModel(x))
                .OrderBy(x => x.RunDate).ToList();
            
            return View(sessionModels);
        }

        [Route("{id}")]
        public async Task<IActionResult> Report(long id)
        {
            var report = await _service.GetDiscoveryReportAsync(id);
            
            return report != null 
                ? View(new DiscoveryReportViewModel(report))
                : View("NotFound", id);
        }
    }
}