using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi.Entities
{
    /// <summary>
    /// Judgements for submissions in the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Judgements">More detail</a>
    /// </summary>
    public class Judgement : EntityBase
    {
        /// <summary>
        /// Identifier of the judgement
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Identifier of the submission judged
        /// </summary>
        [JsonProperty("submission_id")]
        public string SubmissionId { get; init; } = null!;

        /// <summary>
        /// The verdict of this judgement
        /// </summary>
        [JsonProperty("judgement_type_id")]
        public string? JudgementTypeId { get; init; }

        /// <summary>
        /// Score for this judgement
        /// </summary>
        /// <remarks>Only relevant if <c>contest:scoreboard_type</c> is <c>score</c>. Defaults to 100 if missing</remarks>
        [JsonProperty("judgement_score", NullValueHandling = NullValueHandling.Ignore)]
        public double? JudgementScore { get; init; }

        /// <summary>
        /// Absolute time when judgement started
        /// </summary>
        [JsonProperty("start_time")]
        public DateTimeOffset StartTime { get; init; }

        /// <summary>
        /// Contest relative time when judgement started
        /// </summary>
        [JsonProperty("start_contest_time")]
        public TimeSpan StartContestTime { get; init; }

        /// <summary>
        /// Absolute time when judgement completed
        /// </summary>
        [JsonProperty("end_time")]
        public DateTimeOffset? EndTime { get; init; }

        /// <summary>
        /// Contest relative time when judgement completed
        /// </summary>
        [JsonProperty("end_contest_time")]
        public TimeSpan? EndContestTime { get; init; }

        /// <summary>
        /// Maximum run time in seconds for any test case
        /// </summary>
        [JsonProperty("max_run_time", NullValueHandling = NullValueHandling.Ignore)]
        public double? MaxRunTime { get; init; }

        /// <summary>
        /// If this judgement is valid and active
        /// </summary>
        /// <remarks>This is not CCS compatible property</remarks>
        [JsonProperty("valid")]
        public bool Valid { get; init; } = true;

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
