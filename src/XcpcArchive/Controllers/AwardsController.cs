using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/awards")]
    public class AwardsController : CosmosControllerBase
    {
        public AwardsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "awards", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Award>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Award>(
                "SELECT c.id, c.citation, c.team_ids " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{awardId}")]
        public async Task<ActionResult<Award?>> GetOne([FromRoute] string id, [FromRoute] string awardId)
        {
            id = id.ToLower().Trim();
            awardId = awardId.Trim();
            return await GetOne<Award>(awardId, id);
        }
    }
}
