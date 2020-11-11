using Microsoft.AspNetCore.Mvc;

namespace Parsia.Areas.Administrator.Controllers.core.person
{
    [Area("Administrator")]
    [Route("person")]
    public class PersonController : Controller
    {
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("edit")]
        public IActionResult Edit()
        {
            return View();
        }
    }
}