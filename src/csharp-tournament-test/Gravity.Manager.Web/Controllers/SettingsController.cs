using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Gravity.Manager.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Gravity.Manager.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
