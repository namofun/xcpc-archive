using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Provides information on the current contest.
    /// </summary>
    /// <remarks>
    /// References:
    /// <list type="bullet">
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Contests">Contest API 2020 (baylor wiki)</a>
    /// </list>
    /// <list type="bullet">
    /// <a href="https://ccs-specs.icpc.io/contest_api">CCS Specs (GitHub)</a>
    /// </list>
    /// </remarks>
    public class Contest : EntityBase
    {
        /// <summary>
        /// Identifier of the current contest
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Short logo name of the contest
        /// </summary>
        [JsonProperty("shortname")]
        public string ShortName { get; init; } = null!;

        /// <summary>
        /// Short display name of the contest
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Full name of the contest
        /// </summary>
        [JsonProperty("formal_name")]
        public string FormalName { get; init; } = null!;

        /// <summary>
        /// The scheduled start time of the contest
        /// </summary>
        /// <remarks>May be <c>null</c> if the start time is unknown or the countdown is paused.</remarks>
        [JsonProperty("start_time")]
        public DateTimeOffset? StartTime { get; init; }

        /// <summary>
        /// Length of the contest
        /// </summary>
        [JsonProperty("duration")]
        public TimeSpan Duration { get; init; }

        /// <summary>
        /// How long the scoreboard is frozen before the end of the contest
        /// </summary>
        [JsonProperty("scoreboard_freeze_duration")]
        public TimeSpan? ScoreboardFreezeDuration { get; init; }

        /// <summary>
        /// Penalty time for a wrong submission, in minutes
        /// </summary>
        [JsonProperty("penalty_time")]
        public int PenaltyTime { get; init; }

        /// <summary>
        /// The scheduled end time of the contest
        /// </summary>
        [JsonProperty("end_time")]
        public DateTimeOffset? EndTime { get; init; }

        /// <summary>
        /// What type of scoreboard is used for the contest
        /// </summary>
        /// <remarks>
        /// Must be either <c>pass-fail</c> or <c>score</c>.
        /// Defaults to <c>pass-fail</c> if missing or <c>null</c>.
        /// </remarks>
        [JsonProperty("scoreboard_type", NullValueHandling = NullValueHandling.Ignore)]
        public string? ScoreboardType { get; init; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
