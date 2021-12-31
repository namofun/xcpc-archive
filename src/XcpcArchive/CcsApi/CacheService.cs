using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XcpcArchive.CcsApi.Entities;

namespace XcpcArchive.CcsApi
{
    public static class CcsApiCacheService
    {
        public static Task<HashSet<string>> CachedGetContestIdsAsync(this CcsApiClient client)
        {
            return client.GetMemoryCache().GetOrCreateAsync(CacheKey.ContestList, async entry =>
            {
                List<JObject> result = await client.GetCustomObjectsAsync<Contest, JObject>("SELECT c._cid FROM c", new { });
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return result.Select(o => (string)o["_cid"]!).ToHashSet();
            });
        }
    }

    public class CacheKey
    {
        public static CacheKey ContestList { get; } = new();
    }
}
