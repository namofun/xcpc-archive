using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
            Contest? contest = await _client.GetEntityAsync<Contest>(id, id);
            return contest == null ? null! : (contest.State ?? new State());
        }
    }
}
