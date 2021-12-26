using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;
using XcpcArchive.CcsApi.Models;

namespace XcpcArchive.Controllers
{
    public class AdministratorController : ApiControllerBase
    {
        [HttpGet("/api/job/{partitionKey}/{coldId}")]
        [Authorize(Roles = "XcpcArchive.Uploader")]
        public async Task<ActionResult<JobEntry?>> GetJob(
            [FromRoute] string partitionKey,
            [FromRoute] string coldId,
            [FromServices] CcsApiClient client)
        {
            partitionKey = client.NormalizeContestId(partitionKey);
            coldId = client.NormalizeContestId(coldId);

            List<JobEntry> entries = await client.GetListAsync<JobEntry>(
                "SELECT * FROM c WHERE c._cid = @partitionKey AND c.externalid = @coldId",
                new { partitionKey, coldId });

            return entries.SingleOrDefault();
        }

        [HttpPost("/api/upload/ccsapi/{id}")]
        [Consumes("application/zip")]
        [Authorize(Roles = "XcpcArchive.Uploader")]
        public async Task<ActionResult<JobEntry>> PostNew(
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
                    : CreatedAtAction(nameof(GetJob), new { entry.PartitionKey, entry.ColdId }, entry);
            }
            else
            {
                return new UnsupportedMediaTypeResult();
            }
        }
    }
}
