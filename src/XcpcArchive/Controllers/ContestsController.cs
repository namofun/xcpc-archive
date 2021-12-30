using Microsoft.AspNetCore.Mvc;

namespace XcpcArchive.Controllers
{
    [Route("/contests")]
    public class ContestsController : Controller
    {
        [HttpGet]
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{id}/registration")]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpGet("{id}/details")]
        public IActionResult Details()
        {
            return View();
        }

        [HttpGet("{id}/clarifications")]
        public IActionResult Clarifications()
        {
            return View();
        }

        [HttpGet("{id}/submissions")]
        public IActionResult Submissions()
        {
            return View();
        }
    }
}
