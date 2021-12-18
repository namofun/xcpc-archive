using System;
using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Judgements for submissions in the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Judgements">More detail</a>
    /// </summary>
    public class Judgement
    {
        /// <summary>
        /// Identifier of the judgement
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Identifier of the submission judged
        /// </summary>
        [JsonPropertyName("submission_id")]
        public string SubmissionId { get; init; } = null!;

        /// <summary>
        /// The verdict of this judgement
        /// </summary>
        [JsonPropertyName("judgement_type_id")]
        public string? JudgementTypeId { get; init; }

        /// <summary>
        /// Score for this judgement
        /// </summary>
        /// <remarks>Only relevant if <c>contest:scoreboard_type</c> is <c>score</c>. Defaults to 100 if missing</remarks>
        [JsonPropertyName("judgement_score")]
        public double? JudgementScore { get; init; }

        /// <summary>
        /// Absolute time when judgement started
        /// </summary>
        [JsonPropertyName("start_time")]
        public DateTimeOffset StartTime { get; init; }

        /// <summary>
        /// Contest relative time when judgement started
        /// </summary>
        [JsonPropertyName("start_contest_time")]
        public TimeSpan StartContestTime { get; init; }

        /// <summary>
        /// Absolute time when judgement completed
        /// </summary>
        [JsonPropertyName("end_time")]
        public DateTimeOffset? EndTime { get; init; }

        /// <summary>
        /// Contest relative time when judgement completed
        /// </summary>
        [JsonPropertyName("end_contest_time")]
        public TimeSpan? EndContestTime { get; init; }

        /// <summary>
        /// Maximum run time in seconds for any test case
        /// </summary>
        [JsonPropertyName("max_run_time")]
        public double? MaxRunTime { get; init; }

        /// <summary>
        /// If this judgement is valid and active
        /// </summary>
        /// <remarks>This is not CCS compatible property</remarks>
        [JsonPropertyName("valid")]
        public bool Valid { get; init; }
    }
}
