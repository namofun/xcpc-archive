using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using XcpcArchive.CcsApi.Entities;

namespace XcpcArchive.CcsApi.Models
{
    public class CacheEntry : EntityBase
    {
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        [JsonProperty("slot")]
        public int Slot { get; init; }

        [JsonProperty("contest")]
        public Contest? Contest { get; init; }

        [JsonProperty("teams", NullValueHandling = NullValueHandling.Ignore)]
        public List<TeamCache>? Teams { get; init; }

        [JsonProperty("problems", NullValueHandling = NullValueHandling.Ignore)]
        public List<ProblemCache>? Problems { get; init; }

        [JsonProperty("judgement-types", NullValueHandling = NullValueHandling.Ignore)]
        public List<JudgementType>? JudgementTypes { get; init; }

        [JsonProperty("submissions", NullValueHandling = NullValueHandling.Ignore)]
        public List<SubmissionCache>? Submissions { get; init; }

        [JsonProperty("_cid")]
        public string ContestId { get; init; } = null!;

        [JsonProperty("_ts")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset LastUpdateTimeStamp { get; init; } = DateTimeOffset.Now;

        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; set; }
    }
}
