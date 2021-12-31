using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi.Entities;
using XcpcArchive.CcsApi.Models;

namespace XcpcArchive.CcsApi
{
    public static class CcsApiCacheGenerator
    {
        const string SelectAllQuery = "SELECT * FROM c WHERE c._cid = @id";

        private static async Task<List<ProblemCache>> GetProblemCacheAsync(CcsApiClient client, Contest contest)
            => ToProblemCache(
                await client.GetListAsync<Problem>(SelectAllQuery, new { id = contest.Id }));

        public static List<ProblemCache> ToProblemCache(IEnumerable<Problem> problems)
        {
            return problems
                .OrderBy(p => p.Ordinal)
                .Select(p => new ProblemCache()
                {
                    Label = p.Label,
                    Id = p.Id,
                    Name = p.Name,
                    Ordinal = p.Ordinal,
                    Rgb = p.Rgb,
                })
                .ToList();
        }

        private static async Task<List<TeamCache>> GetTeamCacheAsync(CcsApiClient client, Contest contest)
            => ToTeamCache(
                await client.GetListAsync<Team>(SelectAllQuery, new { id = contest.Id }),
                await client.GetListAsync<Group>(SelectAllQuery, new { id = contest.Id }),
                await client.GetListAsync<Organization>(SelectAllQuery, new { id = contest.Id }));

        public static List<TeamCache> ToTeamCache(IEnumerable<Team> teams, IEnumerable<Group> groups, IEnumerable<Organization> organizations)
        {
            var orgMap = organizations.ToDictionary(o => o.Id);
            var groupMap = groups.ToDictionary(g => g.Id);

            return teams
                .OrderBy(t => t.Id.Length)
                .ThenBy(t => t.Id)
                .Select(t =>
                {
                    var gps = t.GroupIds
                        .Where(g => g != null)
                        .Select(g => groupMap.GetValueOrDefault(g, Group.Unknown));

                    return new TeamCache()
                    {
                        Id = t.Id,
                        Name = string.IsNullOrEmpty(t.DisplayName) ? t.Name : t.DisplayName,
                        Organization = t.OrganizationId == null ? null : orgMap.GetValueOrDefault(t.OrganizationId)?.Name,
                        Groups = gps.Select(g => g.Name).Where(n => n != null).ToArray(),
                        Hidden = gps.Aggregate(false, (l, r) => l || r.Hidden),
                    };
                })
                .ToList();
        }

        private static async Task<List<SubmissionCache>> GetSubmissionCacheAsync(CcsApiClient client, Contest contest)
            => ToSubmissionCache(
                await client.GetListAsync<Submission>(SelectAllQuery, new { id = contest.Id }),
                await client.GetListAsync<Judgement>(SelectAllQuery, new { id = contest.Id }));

        public static List<SubmissionCache> ToSubmissionCache(IEnumerable<Submission> submits, IEnumerable<Judgement> judges)
        {
            ILookup<string, Judgement> judgements = judges.Where(j => j.Valid).ToLookup(j => j.SubmissionId);
            return submits
                .OrderBy(s => s.Id.Length)
                .ThenBy(s => s.Id)
                .Select(s =>
                {
                    Judgement? j = judgements[s.Id]
                        .OrderByDescending(jj => jj.Id.Length)
                        .ThenByDescending(jj => jj.Id)
                        .FirstOrDefault();

                    return new SubmissionCache()
                    {
                        SubmissionId = s.Id,
                        ContestTime = (int)s.ContestTime.TotalSeconds,
                        JudgementId = j?.Id,
                        JudgementTypeId = j?.JudgementTypeId,
                        ProblemId = s.ProblemId,
                        TeamId = s.TeamId,
                    };
                })
                .ToList();
        }

        public static async Task GenerateCacheAsync(this CcsApiClient client, Contest contest)
            => await GenerateCacheAsync(
                client.GetDatabase().GetContainer("cache"),
                contest,
                await GetProblemCacheAsync(client, contest),
                await GetTeamCacheAsync(client, contest),
                await GetSubmissionCacheAsync(client, contest),
                await client.GetListAsync<JudgementType>("SELECT c.id, c.name, c.penalty, c.solved FROM c WHERE c._cid = @id ORDER BY c.id", new { id = contest.Id }),
                await client.GetCacheAsync<string>("SELECT VALUE c.id FROM c WHERE c._cid = @id", new { id = contest.Id }));

        public static async Task GenerateCacheAsync(
            Container container,
            Contest contest,
            List<ProblemCache> problemCache,
            List<TeamCache> teamCache,
            List<SubmissionCache> submissionCache,
            List<JudgementType> judgementTypes,
            List<string> cacheKeys)
        {
            List<CacheEntry> generatedCache = new();

            generatedCache.Add(new()
            {
                Id = contest.Id + "--slot-0",
                Slot = 0,
                Problems = problemCache,
                Teams = teamCache,
                JudgementTypes = judgementTypes,
                Contest = contest,
                ContestId = contest.Id,
            });

            generatedCache.AddRange(submissionCache.Chunk(1000).Select((ss, i) => new CacheEntry()
            {
                Id = contest.Id + "--slot-" + (i + 1),
                Slot = i + 1,
                ContestId = contest.Id,
                Submissions = ss.ToList(),
            }));

            TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(contest.Id));
            foreach (string unusedKey in cacheKeys.Except(generatedCache.Select(j => j.Id)))
            {
                batch.DeleteItem(unusedKey);
            }

            foreach (CacheEntry entry in generatedCache)
            {
                batch.UpsertItem(entry);
            }

            await batch.ExecuteAsync();
        }
    }
}
