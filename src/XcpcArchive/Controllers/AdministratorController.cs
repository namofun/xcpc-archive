using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    public class AdministratorController : CosmosControllerBase
    {
        public AdministratorController(
            CosmosClient cosmosClient,
            ILogger<AdministratorController> logger)
            : base(cosmosClient, "ccsapi", "uploaded-jobs", logger)
        {
        }

        [HttpGet("/api/job/{partitionKey}/{coldId}")]
        [Authorize(Roles = "XcpcArchive.Uploader")]
        public async Task<ActionResult<JobEntry?>> GetJob(
            [FromRoute] string partitionKey,
            [FromRoute] string coldId)
        {
            List<JobEntry> entries = await GetSql<JobEntry>(
                "SELECT * FROM c WHERE c._cid = @partitionKey AND c.externalid = @coldId",
                new { partitionKey, coldId });

            return entries.SingleOrDefault();
        }

        [HttpPost("/api/upload/ccsapi/{id}")]
        [Consumes("application/zip")]
        [Authorize(Roles = "XcpcArchive.Uploader")]
        public async Task<ActionResult<Contest>> PostNew(
            [FromRoute] string id,
            [FromServices] CcsApiImportService importService)
        {
            if (!Request.ContentLength.HasValue)
            {
                return StatusCode((int)System.Net.HttpStatusCode.LengthRequired);
            }
            else if (Request.ContentType == "application/zip")
            {
                id = id.ToLower().Trim();
                byte[] memory = await Request.Body.ReadAllAsync(Request.ContentLength);
                JobEntry? entry = await importService.EnqueueAsync(id, memory);

                return entry == null
                    ? Conflict(new { result = "Job with same id already exists." })
                    : CreatedAtAction(nameof(GetJob), entry);
            }
            else
            {
                return new UnsupportedMediaTypeResult();
            }
        }
    }
}
