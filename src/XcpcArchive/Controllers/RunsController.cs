using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/runs")]
    public class RunsController : CosmosControllerBase
    {
        public RunsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "runs", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Run>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Run>(
                "SELECT c.id, c.judgement_id, c.ordinal," +
                      " c.judgement_type_id, c.time, c.contest_time " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{runId}")]
        public async Task<ActionResult<Run?>> GetOne([FromRoute] string id, [FromRoute] string runId)
        {
            id = id.ToLower().Trim();
            runId = runId.Trim();
            return await GetOne<Run>(runId, id);
        }
    }
}
