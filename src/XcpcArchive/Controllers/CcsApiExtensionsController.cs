using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;
using XcpcArchive.CcsApi.Entities;
using XcpcArchive.CcsApi.Models;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests/{id}/extensions")]
    public class CcsApiExtensionsController : ApiControllerBase
    {
        private readonly CcsApiClient _client;

        public CcsApiExtensionsController(CcsApiClient client)
        {
            _client = client;
        }

        [HttpPost("refresh-cache")]
        [Authorize(Roles = "XcpcArchive.Uploader")]
        public async Task<IActionResult> RefreshCache([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            Contest? contest = await _client.GetEntityAsync<Contest>(id, id);
            if (contest == null)
            {
                return NotFound(new { comment = "No such contest matching provided ID." });
            }

            await _client.GenerateCacheAsync(contest);
            return Ok(new { comment = "Cache has been successfully refreshed." });
        }

        [HttpGet("board-xcpcio-com/run.json")]
        public async Task<IActionResult> BoardXcpcioRunJson([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            List<CacheEntry> caches = await _client.GetCacheAsync<CacheEntry>(
                "SELECT * FROM c WHERE c._cid = @id ORDER BY c.slot",
                new { id });

            Response.Headers.Add(
                "x-cache-slots",
                caches.Select(c => $"{c.Id}|{c.LastUpdateTimeStamp.ToUnixTimeSeconds()}").ToArray());

            JArray array = new JArray();
            if (caches.Count > 0 && caches[0].Id == id + "--slot-0")
            {
                Dictionary<string, int> probMap = caches[0].Problems!
                    .ToDictionary(c => c.Id, c => c.Ordinal);

                HashSet<string> visibleTeams = caches[0].Teams!
                    .Where(t => !t.Hidden)
                    .Select(t => t.Id)
                    .ToHashSet();

                foreach (SubmissionCache s in caches.Skip(1).SelectMany(c => c.Submissions!))
                {
                    if (visibleTeams.Contains(s.TeamId))
                    {
                        array.Add(JObject.FromObject(new
                        {
                            team_id = s.TeamId,
                            problem_id = probMap[s.ProblemId],
                            timestamp = s.ContestTime,
                            status = s.JudgementTypeId == null
                                ? "pending"
                                : s.JudgementTypeId == "AC"
                                ? "correct"
                                : "incorrect",
                        }));
                    }
                }
            }

            return new JsonResult(array);
        }
    }
}
