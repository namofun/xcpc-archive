using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/problems")]
    public class ProblemsController : CosmosControllerBase
    {
        public ProblemsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "problems", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Problem>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Problem>(
                "SELECT c.id, c.label, c.name, c.ordinal, c.time_limit," +
                      " c.rgb, c.color, c.test_data_count " +
                "FROM c WHERE c._cid = @id ORDER BY c.ordinal",
                new { id });
        }

        [HttpGet("{problemId}")]
        public async Task<ActionResult<Problem?>> GetOne([FromRoute] string id, [FromRoute] string problemId)
        {
            id = id.ToLower().Trim();
            problemId = problemId.Trim();
            return await GetOne<Problem>(problemId, id);
        }
    }
}
