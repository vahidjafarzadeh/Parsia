using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;

namespace Parsia.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    [Route("admin")]
    public class ManagementController : Controller
    {

        public IActionResult Index()
        {
            ViewData["Title"] = SystemConfig.AdminTitlePage;
            ViewData["UserImage"] = "avatar.png";
            return View();
        }
    }
}