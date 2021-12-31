using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
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

            if (caches.Count == 0 || caches[0].Id != id + "--slot-0")
                return StatusCode((int)System.Net.HttpStatusCode.ServiceUnavailable);

            JArray array = new JArray();

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

            return new JsonResult(array);
        }

        [HttpGet("/api/contests/{id}/scoreboard")]
        public async Task<ActionResult<Scoreboard>> GetScoreboard([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            List<CacheEntry> caches = await _client.GetCacheAsync<CacheEntry>(
                "SELECT * FROM c WHERE c._cid = @id ORDER BY c.slot",
                new { id });

            Response.Headers.Add(
                "x-cache-slots",
                caches.Select(c => $"{c.Id}|{c.LastUpdateTimeStamp.ToUnixTimeSeconds()}").ToArray());

            if (caches.Count == 0 || caches[0].Id != id + "--slot-0")
                return StatusCode((int)System.Net.HttpStatusCode.ServiceUnavailable);

            Dictionary<string, string> visibleTeams = caches[0].Teams!.Where(t => !t.Hidden).ToDictionary(t => t.Id, t => t.Name);
            Dictionary<string, int> probMap = caches[0].Problems!.ToDictionary(p => p.Id, p => p.Ordinal);
            Dictionary<string, JudgementType> jtMap = caches[0].JudgementTypes!.ToDictionary(k => k.Id);
            Dictionary<string, Scoreboard.Row> rows = new();
            bool[] isSolved = new bool[probMap.Count];

            foreach (SubmissionCache s in caches.Skip(1).SelectMany(c => c.Submissions!))
            {
                if (!visibleTeams.ContainsKey(s.TeamId)) continue;
                if (s.ContestTime >= caches[0].Contest!.Duration.TotalSeconds) continue;

                if (!rows.TryGetValue(s.TeamId, out Scoreboard.Row? row))
                {
                    rows.Add(s.TeamId, row = new()
                    {
                        TeamId = s.TeamId,
                        Problems = new Scoreboard.Problem[probMap.Count],
                        Score =
                        {
                            NumSolved = 0,
                            TotalTime = 0,
                        },
                    });

                    for (int i = 0; i < probMap.Count; i++)
                    {
                        row.Problems[i] = new()
                        {
                            ProblemId = caches[0].Problems![i].Id,
                            Label = caches[0].Problems![i].Label,
                            Solved = false,
                        };
                    }
                }

                int probIdx = probMap[s.ProblemId];
                if (row.Problems[probIdx].Solved ?? false)
                {
                    continue;
                }
                else if (s.JudgementTypeId == null)
                {
                    row.Problems[probIdx].NumPending++;
                }
                else if (jtMap[s.JudgementTypeId].Solved)
                {
                    row.Problems[probIdx].NumJudged++;
                    row.Problems[probIdx].Solved = true;
                    row.Problems[probIdx].Time = s.ContestTime / 60;
                    row.Problems[probIdx].FirstToSolve = !isSolved[probIdx];
                    isSolved[probIdx] = true;

                    row.Score.LastAccepted = s.ContestTime / 60;
                    row.Score.NumSolved++;
                    row.Score.TotalTime += s.ContestTime / 60 + caches[0].Contest!.PenaltyTime * (row.Problems[probIdx].NumJudged - 1);
                }
                else if (jtMap[s.JudgementTypeId].Penalty)
                {
                    row.Problems[probIdx].NumJudged++;
                }
            }

            List<Scoreboard.Row> rows2 = rows.Values
                .OrderByDescending(x => x.Score.NumSolved)
                .ThenBy(x => x.Score.TotalTime)
                .ThenBy(x => x.Score.LastAccepted)
                .ThenBy(x => visibleTeams[x.TeamId])
                .ToList();

            for (int i = 0; i < rows2.Count; i++)
            {
                if (i > 0 && rows2[i - 1].Score.Equals(rows2[i].Score))
                {
                    rows2[i].Rank = rows2[i - 1].Rank;
                }
                else
                {
                    rows2[i].Rank = i + 1;
                }

                rows2[i].Problems = rows2[i].Problems
                    .Where(p => p.NumJudged + p.NumPending > 0)
                    .ToArray();
            }

            DateTimeOffset? frozen = caches[0].Contest?.EndTime - caches[0].Contest?.ScoreboardFreezeDuration;
            State state = caches[0].Contest?.State ?? new()
            {
                Started = caches[0].Contest?.StartTime,
                Ended = caches[0].Contest?.EndTime,
                Frozen = frozen,
                Thawed = frozen.HasValue ? caches[0].LastUpdateTimeStamp : default,
                Finalized = frozen.HasValue ? caches[0].LastUpdateTimeStamp : default,
            };

            return new Scoreboard()
            {
                Rows = rows2,
                State = state,
                Time = caches[0].LastUpdateTimeStamp,
                ContestTime = caches[0].LastUpdateTimeStamp - state.Started!.Value,
            };
        }

        [HttpGet("fast-cds/report")]
        public async Task<IActionResult> FastCdsReport([FromRoute] string id)
        {
            static void IncAt<T>(Dictionary<T, int> target, T key) where T : notnull
                => target[key] = target.GetValueOrDefault(key) + 1;

            id = _client.NormalizeContestId(id);
            List<CacheEntry> caches = await _client.GetCacheAsync<CacheEntry>(
                "SELECT * FROM c WHERE c._cid = @id ORDER BY c.slot",
                new { id });

            Response.Headers.Add(
                "x-cache-slots",
                caches.Select(c => $"{c.Id}|{c.LastUpdateTimeStamp.ToUnixTimeSeconds()}").ToArray());

            if (caches.Count == 0 || caches[0].Id != id + "--slot-0")
                return StatusCode((int)System.Net.HttpStatusCode.ServiceUnavailable);

            Dictionary<string, JudgementType> jtm = caches[0].JudgementTypes!.ToDictionary(k => k.Id);
            Dictionary<(string, string?), int> language = new(), problem = new();

            HashSet<string> visibleTeams = caches[0].Teams!.Where(t => !t.Hidden).Select(t => t.Id).ToHashSet();
            foreach (SubmissionCache s in caches.Skip(1).SelectMany(c => c.Submissions!))
            {
                if (!visibleTeams.Contains(s.TeamId)) continue;
                IncAt(problem, (s.ProblemId, s.JudgementTypeId));
                IncAt(language, (s.LanguageId, s.JudgementTypeId));
                IncAt(problem, (s.ProblemId, "total"));
                IncAt(language, (s.LanguageId, "total"));

                if (s.JudgementTypeId == null)
                {
                    // Nothing to do here
                }
                else if (jtm[s.JudgementTypeId].Solved)
                {
                    IncAt(problem, (s.ProblemId, "solved"));
                    IncAt(language, (s.LanguageId, "solved"));
                }
                else if (jtm[s.JudgementTypeId].Penalty)
                {
                    IncAt(problem, (s.ProblemId, "failed"));
                    IncAt(language, (s.LanguageId, "failed"));
                }
            }

            List<JObject> runReport = await _client.GetCustomObjectsAsync<Run, JObject>(
                "SELECT c.judgement_type_id AS type, COUNT(1) AS count FROM c " +
                "WHERE c._cid = @id AND c.contest_time >= \"0:00:00.000\" " +
                "GROUP BY c.judgement_type_id",
                new { id });

            List<ProblemCache> problems = caches[0].Problems!;
            List<Language> languages = await _client.GetListAsync<Language>(
                "SELECT c.id, c.name FROM c WHERE c._cid = @id ORDER BY c.id",
                new { id });

            Dictionary<string, int> run = runReport.ToDictionary(o => (string)o["type"]!, o => (int)o["count"]!);

            List<JObject> langs = new(), runs = new(), probs = new();
            runs.Add(JObject.FromObject(new { id = "ID", Id = "Id", name = "Name", total = "Total" }));
            langs.AddRange(languages.Select(l => JObject.FromObject(new { id = l.Id , name = l.Name })).Prepend(JObject.FromObject(new { id = "ID", name = "Name" })));
            probs.AddRange(problems.Select(l => JObject.FromObject(new { id = l.Id, name = l.Name })).Prepend(JObject.FromObject(new { id = "ID", name = "Name" })));
            foreach (JudgementType jt in caches[0].JudgementTypes!)
            {
                runs.Add(JObject.FromObject(new { id = jt.Id, jt.Id, name = jt.Name, total = run.GetValueOrDefault(jt.Id).ToString() }));
            }

            foreach ((string Id, string Name) in caches[0].JudgementTypes!.Select(j => (j.Id, j.Id)).Append(("solved", "Solved")).Append(("failed", "Failed")).Append(("total", "Total")))
            {
                langs[0][Id] = probs[0][Id] = Name;
                for (int i = 0; i < languages.Count; i++) langs[i + 1][Id] = language.GetValueOrDefault((languages[i].Id, Id)).ToString();
                for (int i = 0; i < problems.Count; i++) probs[i + 1][Id] = problem.GetValueOrDefault((problems[i].Id, Id)).ToString();
            }

            return new JsonResult(new { runs, langs, probs });
        }
    }
}
