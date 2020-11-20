using System;
using System.Diagnostics;
using DataLayer.Token;
using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Parsia.Core.SiteMap;
using Parsia.Models;

namespace Parsia.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostEnvironment _env;

        public HomeController(IHostEnvironment env)
        {
            _env = env;
        }

        

        public IActionResult Index()
        {
            SiteMap.GenerateSiteMap("www.test.com",0.1,DateTime.Now,1,_env.ContentRootPath+"/"+SystemConfig.Root+"/"+"sitemap/article/");
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