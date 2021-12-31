using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

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

        [HttpGet("{id}")]
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

        [HttpGet("{id}/scoreboard")]
        public IActionResult Scoreboard()
        {
            return View();
        }

        [HttpGet("{id}/reports")]
        public IActionResult Reports()
        {
            return View();
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (RouteData.Values.TryGetValue("id", out object? idobj)
                && idobj is string cid)
            {
                CcsApiClient client = HttpContext.RequestServices.GetRequiredService<CcsApiClient>();
                HashSet<string> cids = await client.CachedGetContestIdsAsync();
                if (!cids.Contains(cid))
                {
                    context.HttpContext.Response.StatusCode = 404;
                    context.Result = View("NotFound");
                }
            }

            if (context.Result == null)
            {
                await next();
            }
        }
    }
}
