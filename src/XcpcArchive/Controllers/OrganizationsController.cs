using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/organizations")]
    public class OrganizationsController : CosmosControllerBase
    {
        public OrganizationsController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "organizations", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Organization>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Organization>(
                "SELECT c.id, c.name, c.formal_name, c.country " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{orgId}")]
        public async Task<ActionResult<Organization?>> GetOne([FromRoute] string id, [FromRoute] string orgId)
        {
            id = id.ToLower().Trim();
            orgId = orgId.Trim();
            return await GetOne<Organization>(orgId, id);
        }
    }
}
