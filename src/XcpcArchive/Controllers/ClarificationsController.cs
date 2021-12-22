using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/clarifications")]
    public class ClarificationsController : CosmosControllerBase
    {
        public ClarificationsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "clarifications", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Clarification>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Clarification>(
                "SELECT c.id, c.from_team_id, c.to_team_id, c.reply_to_id," +
                      " c.problem_id, c.text, c.time, c.contest_time " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{clarId}")]
        public async Task<ActionResult<Clarification?>> GetOne([FromRoute] string id, [FromRoute] string clarId)
        {
            id = id.ToLower().Trim();
            clarId = clarId.Trim();
            return await GetOne<Clarification>(clarId, id);
        }
    }
}
