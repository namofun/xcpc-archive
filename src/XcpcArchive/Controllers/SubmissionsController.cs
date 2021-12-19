using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/submissions")]
    public class SubmissionsController : CosmosControllerBase
    {
        public SubmissionsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "submissions", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Submission>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Submission>(
                new QueryDefinition("SELECT c.id, c.language_id, c.problem_id, c.team_id, c.time, c.contest_time, c.entry_point, c.files FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC")
                    .WithParameter("@id", id));
        }

        [HttpGet("{submissionId}")]
        public async Task<ActionResult<Submission?>> GetOne([FromRoute] string id, [FromRoute] string submissionId)
        {
            id = id.ToLower().Trim();
            submissionId = submissionId.ToLower().Trim();
            return await GetOne<Submission>(submissionId, id);
        }
    }
}
