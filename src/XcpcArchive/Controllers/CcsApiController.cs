using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi;
using XcpcArchive.CcsApi.Entities;
using XcpcArchive.CcsApi.Models;

namespace XcpcArchive.Controllers
{
    [Route("/api/contests")]
    public class CcsApiController : ApiControllerBase
    {
        private readonly CcsApiClient _client;

        public CcsApiController(CcsApiClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult<List<Contest>>> GetContests()
        {
            return await _client.GetListAsync<Contest>(
                "SELECT c.formal_name, c.penalty_time, c.start_time, c.end_time, c.duration," +
                      " c.scoreboard_freeze_duration, c.id, c.name, c.shortname, c._statistics " +
                "FROM c ORDER BY c.start_time DESC");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contest?>> GetContest([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetEntityAsync<Contest>(id, id);
        }

        [HttpGet("{id}/awards")]
        public async Task<ActionResult<List<Award>>> GetAwards([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Award>(
                "SELECT c.id, c.citation, c.team_ids " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/awards/{awardId}")]
        public async Task<ActionResult<Award?>> GetAward([FromRoute] string id, [FromRoute] string awardId)
        {
            id = _client.NormalizeContestId(id);
            awardId = awardId.Trim();
            return await _client.GetEntityAsync<Award>(awardId, id);
        }

        [HttpGet("{id}/clarifications")]
        public async Task<ActionResult<List<Clarification>>> GetClarifications([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Clarification>(
                "SELECT c.id, c.from_team_id, c.to_team_id, c.reply_to_id," +
                      " c.problem_id, c.text, c.time, c.contest_time " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/clarifications/{clarId}")]
        public async Task<ActionResult<Clarification?>> GetClarification([FromRoute] string id, [FromRoute] string clarId)
        {
            id = _client.NormalizeContestId(id);
            clarId = clarId.Trim();
            return await _client.GetEntityAsync<Clarification>(clarId, id);
        }

        [HttpGet("{id}/groups")]
        public async Task<ActionResult<List<Group>>> GetGroups([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Group>(
                "SELECT c.id, c.name, c.type, c.hidden " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/groups/{groupId}")]
        public async Task<ActionResult<Group?>> GetGroup([FromRoute] string id, [FromRoute] string groupId)
        {
            id = _client.NormalizeContestId(id);
            groupId = groupId.Trim();
            return await _client.GetEntityAsync<Group>(groupId, id);
        }

        [HttpGet("{id}/judgements")]
        public async Task<ActionResult<List<Judgement>>> GetJudgements([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Judgement>(
                "SELECT c.id, c.submission_id, c.judgement_type_id, c.judgement_score," +
                      " c.start_time, c.start_contest_time, c.end_time," +
                      " c.end_contest_time, c.max_run_time, c.valid " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/judgements/{judgementId}")]
        public async Task<ActionResult<Judgement?>> GetJudgement([FromRoute] string id, [FromRoute] string judgementId)
        {
            id = _client.NormalizeContestId(id);
            judgementId = judgementId.Trim();
            return await _client.GetEntityAsync<Judgement>(judgementId, id);
        }

        [HttpGet("{id}/judgement-types")]
        public async Task<ActionResult<List<JudgementType>>> GetJudgementTypes([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<JudgementType>(
                "SELECT c.id, c.name, c.penalty, c.solved " +
                "FROM c WHERE c._cid = @id ORDER BY c.id",
                new { id });
        }

        [HttpGet("{id}/judgement-types/{typeId}")]
        public async Task<ActionResult<JudgementType?>> GetJudgementType([FromRoute] string id, [FromRoute] string typeId)
        {
            id = _client.NormalizeContestId(id);
            typeId = typeId.Trim();
            return await _client.GetEntityAsync<JudgementType>(typeId, id);
        }

        [HttpGet("{id}/languages")]
        public async Task<ActionResult<List<Language>>> GetLanguages([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Language>(
                "SELECT c.id, c.name, c.extensions, c.require_entry_point " +
                "FROM c WHERE c._cid = @id ORDER BY c.id",
                new { id });
        }

        [HttpGet("{id}/languages/{langId}")]
        public async Task<ActionResult<Language?>> GetLanguage([FromRoute] string id, [FromRoute] string langId)
        {
            id = _client.NormalizeContestId(id);
            langId = langId.Trim();
            return await _client.GetEntityAsync<Language>(langId, id);
        }

        [HttpGet("{id}/organizations")]
        public async Task<ActionResult<List<Organization>>> GetOrganizations([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Organization>(
                "SELECT c.id, c.name, c.formal_name, c.country " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/organizations/{orgId}")]
        public async Task<ActionResult<Organization?>> GetOrganization([FromRoute] string id, [FromRoute] string orgId)
        {
            id = _client.NormalizeContestId(id);
            orgId = orgId.Trim();
            return await _client.GetEntityAsync<Organization>(orgId, id);
        }

        [HttpGet("{id}/problems")]
        public async Task<ActionResult<List<Problem>>> GetProblems([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Problem>(
                "SELECT c.id, c.label, c.name, c.ordinal, c.time_limit," +
                      " c.rgb, c.color, c.test_data_count " +
                "FROM c WHERE c._cid = @id ORDER BY c.ordinal",
                new { id });
        }

        [HttpGet("{id}/problems/{problemId}")]
        public async Task<ActionResult<Problem?>> GetProblem([FromRoute] string id, [FromRoute] string problemId)
        {
            id = id.ToLower().Trim();
            problemId = problemId.Trim();
            return await _client.GetEntityAsync<Problem>(problemId, id);
        }

        [HttpGet("{id}/runs")]
        public async Task<ActionResult<List<Run>>> GetRuns([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Run>(
                "SELECT c.id, c.judgement_id, c.ordinal," +
                      " c.judgement_type_id, c.time, c.contest_time " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/runs/{runId}")]
        public async Task<ActionResult<Run?>> GetRun([FromRoute] string id, [FromRoute] string runId)
        {
            id = _client.NormalizeContestId(id);
            runId = runId.Trim();
            return await _client.GetEntityAsync<Run>(runId, id);
        }

        [HttpGet("{id}/submissions")]
        public async Task<ActionResult<List<Submission>>> GetSubmissions([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Submission>(
                "SELECT c.id, c.language_id, c.problem_id, c.team_id," +
                      " c.time, c.contest_time, c.entry_point, c.files " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/submissions/{submitId}")]
        public async Task<ActionResult<Submission?>> GetSubmission([FromRoute] string id, [FromRoute] string submitId)
        {
            id = _client.NormalizeContestId(id);
            submitId = submitId.Trim();
            Submission? submission = await _client.GetEntityAsync<Submission>(submitId, id);
            submission?.TruncateSingleOutput();
            return submission;
        }

        [HttpGet("{id}/submissions/{submitId}/files")]
        [Produces("application/zip")]
        public async Task<IActionResult> GetSubmissionFile([FromRoute] string id, [FromRoute] string submitId)
        {
            id = _client.NormalizeContestId(id);
            submitId = submitId.Trim();
            Submission? submission = await _client.GetEntityAsync<Submission>(submitId, id);

            return submission?.FileContent != null
                ? File(submission.FileContent, "application/zip")
                : NotFound();
        }

        [HttpGet("{id}/teams")]
        public async Task<ActionResult<List<Team>>> GetTeams([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            return await _client.GetListAsync<Team>(
                "SELECT c.id, c.icpc_id, c.name, c.display_name," +
                      " c.organization_id, c.group_ids " +
                "FROM c WHERE c._cid = @id ORDER BY c._idlen ASC, c.id ASC",
                new { id });
        }

        [HttpGet("{id}/teams/{teamId}")]
        public async Task<ActionResult<Team?>> GetTeam([FromRoute] string id, [FromRoute] string teamId)
        {
            id = _client.NormalizeContestId(id);
            teamId = teamId.Trim();
            return await _client.GetEntityAsync<Team>(teamId, id);
        }

        [HttpGet("{id}/state")]
        public async Task<ActionResult<State>> GetState([FromRoute] string id)
        {
            id = _client.NormalizeContestId(id);
            CacheEntry? cache = await _client.GetEntityAsync<CacheEntry>(id + "--slot-0", id);
            DateTimeOffset? frozen = cache?.Contest?.EndTime - cache?.Contest?.ScoreboardFreezeDuration;
            State state = cache?.Contest?.State ?? new State()
            {
                Started = cache?.Contest?.StartTime,
                Ended = cache?.Contest?.EndTime,
                Frozen = frozen,
                Thawed = frozen.HasValue ? cache?.LastUpdateTimeStamp : default,
                Finalized = frozen.HasValue ? cache?.LastUpdateTimeStamp : default,
            };

            return state;
        }

        [HttpGet("{id}/scoreboard")]
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
                throw new NotImplementedException();

            HashSet<string> visibleTeams = caches[0].Teams!.Where(t => !t.Hidden).Select(t => t.Id).ToHashSet();
            Dictionary<string, int> probMap = caches[0].Problems!.ToDictionary(p => p.Id, p => p.Ordinal);
            Dictionary<string, JudgementType> jtMap = caches[0].JudgementTypes!.ToDictionary(k => k.Id);
            Dictionary<string, Scoreboard.Row> rows = new();
            bool[] isSolved = new bool[probMap.Count];

            foreach (SubmissionCache s in caches.Skip(1).SelectMany(c => c.Submissions!))
            {
                if (!visibleTeams.Contains(s.TeamId)) continue;
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
    }
}
