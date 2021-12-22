using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/groups")]
    public class GroupsController : CosmosControllerBase
    {
        public GroupsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "groups", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Group>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Group>(
                "SELECT c.id, c.name, c.type, c.hidden " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{groupId}")]
        public async Task<ActionResult<Group?>> GetOne([FromRoute] string id, [FromRoute] string groupId)
        {
            id = id.ToLower().Trim();
            groupId = groupId.Trim();
            return await GetOne<Group>(groupId, id);
        }
    }
}
