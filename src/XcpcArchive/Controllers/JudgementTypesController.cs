using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/judgement-types")]
    public class JudgementTypesController : CosmosControllerBase
    {
        public JudgementTypesController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "judgement-types", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<JudgementType>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<JudgementType>(
                "SELECT c.id, c.name, c.penalty, c.solved " +
                "FROM c WHERE c._cid = @id ORDER BY c.id",
                new { id });
        }

        [HttpGet("{typeId}")]
        public async Task<ActionResult<JudgementType?>> GetOne([FromRoute] string id, [FromRoute] string typeId)
        {
            id = id.ToLower().Trim();
            typeId = typeId.Trim();
            return await GetOne<JudgementType>(typeId, id);
        }
    }
}
