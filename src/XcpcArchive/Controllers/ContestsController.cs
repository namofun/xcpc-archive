using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests")]
    public class ContestsController : CosmosControllerBase
    {
        public ContestsController(
            CosmosClient cosmosClient,
            ILogger<ContestsController> logger)
            : base(cosmosClient, "ccsapi", "contests", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Contest>>> GetAll()
        {
            return await GetSql<Contest>(
                "SELECT c.formal_name, c.penalty_time, c.start_time, c.end_time, c.duration," +
                      " c.scoreboard_freeze_duration, c.id, c.name, c.shortname, c._statistics " +
                "FROM c ORDER BY c.start_time DESC");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contest?>> GetOne([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetOne<Contest>(id, id);
        }
    }
}
