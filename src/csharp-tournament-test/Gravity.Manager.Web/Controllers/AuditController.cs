using System;
using System.Threading.Tasks;
using Gravity.Manager.ApplicationService;
using Gravity.Manager.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gravity.Manager.Web.Controllers
{
    [Authorize]
    public class AuditController : Controller
    {
        private const int PageSize = 10;
        
        private readonly IAuditAppService _service;

        public AuditController(IAuditAppService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        public async Task<IActionResult> Index(int? page)
        {
            var pageData = await _service.GetAuditPageAsync(page ?? 0, PageSize);
            
            return View(pageData);
        }
    }
}