using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/judgements")]
    public class JudgementsController : CosmosControllerBase
    {
        public JudgementsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "judgements", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Judgement>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Judgement>(
                "SELECT c.id, c.submission_id, c.judgement_type_id, c.judgement_score," +
                      " c.start_time, c.start_contest_time, c.end_time," +
                      " c.end_contest_time, c.max_run_time, c.valid " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{judgementId}")]
        public async Task<ActionResult<Judgement?>> GetOne([FromRoute] string id, [FromRoute] string judgementId)
        {
            id = id.ToLower().Trim();
            judgementId = judgementId.Trim();
            return await GetOne<Judgement>(judgementId, id);
        }
    }
}
