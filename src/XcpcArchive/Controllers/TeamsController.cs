using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/teams")]
    public class TeamsController : CosmosControllerBase
    {
        public TeamsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "teams", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Team>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Team>(
                new QueryDefinition("SELECT c.id, c.icpc_id, c.name, c.display_name, c.organization_id, c.group_ids FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC")
                    .WithParameter("@id", id));
        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<Team?>> GetOne([FromRoute] string id, [FromRoute] string teamId)
        {
            id = id.ToLower().Trim();
            teamId = teamId.ToLower().Trim();
            return await GetOne<Team>(teamId, id);
        }
    }
}
