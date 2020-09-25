using System.Diagnostics;
using DataLayer.Token;
using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Parsia.Models;

namespace Parsia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IJwtHandlers _jwtHandlers;

        public HomeController(ILogger<HomeController> logger, IJwtHandlers jwtHandlers)
        {
            _logger = logger;
            _jwtHandlers = jwtHandlers;
        }

        public IActionResult Index()
        {
            var x = new TokenOption()
            {
                UserInfo = new UserInfo()
                {
                    PersonId = 1,
                    UserId = 1,
                    LastName = "vahid",
                    Picture = "fergregrsdfdfsdfsdfsdfsdfsdfdsf",
                    Username = "vahidjafarzadeh",
                    FirstName = "jafarzadeh",
                    Phone = "efrfrefrf"
                },
                ExpireMinutes = 10,
                Ip = "1515.15151.6262.23"
            };

            InitialToken initialToken = new InitialToken(_jwtHandlers);
            var jsonWebToken = initialToken.GetToken(x);
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