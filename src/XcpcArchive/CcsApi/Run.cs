using System;
using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Runs are judgements of individual test cases of a submission.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Runs">More detail</a>
    /// </summary>
    public class Run
    {
        /// <summary>
        /// Identifier of the run
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Identifier of the judgement this is part of
        /// </summary>
        [JsonPropertyName("judgement_id")]
        public string JudgementId { get; init; } = null!;

        /// <summary>
        /// Ordering of runs in the judgement
        /// </summary>
        /// <remarks>Must be different for every run in a judgement. Runs for the same test case must have the same ordinal. Must be between 1 and problem:<c>test_data_count</c>.</remarks>
        [JsonPropertyName("ordinal")]
        public int Ordinal { get; init; }

        /// <summary>
        /// The verdict of this judgement (i.e. a judgement type)
        /// </summary>
        [JsonPropertyName("judgement_type_id")]
        public string JudgementTypeId { get; init; } = null!;

        /// <summary>
        /// Absolute time when run completed
        /// </summary>
        [JsonPropertyName("time")]
        public DateTimeOffset Time { get; init; }

        /// <summary>
        /// Contest relative time when run completed
        /// </summary>
        [JsonPropertyName("contest_time")]
        public TimeSpan ContestTime { get; init; }

        /// <summary>
        /// Run time in seconds
        /// </summary>
        [JsonPropertyName("run_time")]
        public double RunTime { get; init; }
    }
}
