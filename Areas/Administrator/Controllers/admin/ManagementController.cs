using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Parsia.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Route("admin")]
    public class ManagementController : Controller
    {
        private readonly SystemConfig _systemConfig;

        public ManagementController(IOptions<SystemConfig> options)
        {
            _systemConfig = options.Value;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _systemConfig.AdminTitlePage;
            ViewData["UserImage"] = "avatar.png";
            return View();
        }
    }
}