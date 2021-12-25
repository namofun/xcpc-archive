using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XcpcArchive.Controllers
{
    public class ApiControllerBase : ControllerBase, IActionFilter
    {
        [NonAction]
        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
        }

        [NonAction]
        public virtual void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult
                && objectResult.Value == null
                && objectResult.StatusCode == 200)
            {
                context.Result = NotFound();
            }
        }
    }
}
