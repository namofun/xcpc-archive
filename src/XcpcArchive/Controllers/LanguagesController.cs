using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/languages")]
    public class LanguagesController : CosmosControllerBase
    {
        public LanguagesController(
            CosmosClient cosmosClient,
            ILogger<ProblemsController> logger)
            : base(cosmosClient, "ccsapi", "languages", logger)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Language>>> GetAll([FromRoute] string id)
        {
            id = id.ToLower().Trim();
            return await GetSql<Language>(
                "SELECT c.id, c.name " +
                "FROM c WHERE c._cid = @id ORDER BY c.id",
                new { id });
        }

        [HttpGet("{langId}")]
        public async Task<ActionResult<Language?>> GetOne([FromRoute] string id, [FromRoute] string langId)
        {
            id = id.ToLower().Trim();
            langId = langId.Trim();
            return await GetOne<Language>(langId, id);
        }
    }
}
