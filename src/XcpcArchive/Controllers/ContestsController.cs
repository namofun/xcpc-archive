using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests")]
    public class ContestsController : CosmosControllerBase
    {
        public ContestsController(CosmosClient cosmosClient)
            : base(cosmosClient, "ccsapi", "contests")
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<Contest>>> OnGet()
        {
            return await GetLinq<Contest>(q => q.OrderByDescending(c => c.StartTime));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contest>> OnGet([FromRoute] string id)
        {
            return await GetOne<Contest>(id, id);
        }
    }
}
