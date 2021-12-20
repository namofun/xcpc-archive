using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace XcpcArchive.Controllers
{
    public class AdministratorController : ControllerBase
    {
        [HttpGet("/api/startup")]
        [Authorize(Roles = "XcpcArchive.Uploader")]
        public async Task<IActionResult> Startup(
            [FromServices] StartupInitializer startupInitializer)
        {
            await startupInitializer.DoWorkAsync();
            return Ok(new { result = "ok" });
        }
    }
}
