using System.Diagnostics;
using DataLayer.Token;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parsia.Models;

namespace Parsia.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJwtHandlers _jwtHandlers;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IJwtHandlers jwtHandlers)
        {
            _logger = logger;
            _jwtHandlers = jwtHandlers;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}