using System;
using System.Text.Json.Serialization;

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
    public class Contest
    {
        /// <summary>
        /// Identifier of the current contest
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Short logo name of the contest
        /// </summary>
        [JsonPropertyName("shortname")]
        public string ShortName { get; init; } = null!;

        /// <summary>
        /// Short display name of the contest
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Full name of the contest
        /// </summary>
        [JsonPropertyName("formal_name")]
        public string FormalName { get; init; } = null!;

        /// <summary>
        /// The scheduled start time of the contest
        /// </summary>
        /// <remarks>May be <c>null</c> if the start time is unknown or the countdown is paused.</remarks>
        [JsonPropertyName("start_time")]
        public DateTimeOffset? StartTime { get; init; }

        /// <summary>
        /// Length of the contest
        /// </summary>
        [JsonPropertyName("duration")]
        public TimeSpan Duration { get; init; }

        /// <summary>
        /// How long the scoreboard is frozen before the end of the contest
        /// </summary>
        [JsonPropertyName("scoreboard_freeze_duration")]
        public TimeSpan? ScoreboardFreezeDuration { get; init; }

        /// <summary>
        /// Penalty time for a wrong submission, in minutes
        /// </summary>
        [JsonPropertyName("penalty_time")]
        public int PenaltyTime { get; init; }

        /// <summary>
        /// The scheduled end time of the contest
        /// </summary>
        [JsonPropertyName("end_time")]
        public DateTimeOffset? EndTime { get; init; }

        /// <summary>
        /// What type of scoreboard is used for the contest
        /// </summary>
        /// <remarks>
        /// Must be either <c>pass-fail</c> or <c>score</c>.
        /// Defaults to <c>pass-fail</c> if missing or <c>null</c>.
        /// </remarks>
        [JsonPropertyName("scoreboard_type")]
        public string? ScoreboardType { get; init; }
    }
}
