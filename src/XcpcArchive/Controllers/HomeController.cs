using Microsoft.AspNetCore.Mvc;

namespace XcpcArchive.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about")]
        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [IgnoreAntiforgeryToken]
        [HttpGet("/error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
