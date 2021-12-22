using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

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
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Identifier of the judgement this is part of
        /// </summary>
        [JsonProperty("judgement_id")]
        public string JudgementId { get; init; } = null!;

        /// <summary>
        /// Ordering of runs in the judgement
        /// </summary>
        /// <remarks>Must be different for every run in a judgement. Runs for the same test case must have the same ordinal. Must be between 1 and problem:<c>test_data_count</c>.</remarks>
        [JsonProperty("ordinal")]
        public int Ordinal { get; init; }

        /// <summary>
        /// The verdict of this judgement (i.e. a judgement type)
        /// </summary>
        [JsonProperty("judgement_type_id")]
        public string JudgementTypeId { get; init; } = null!;

        /// <summary>
        /// Absolute time when run completed
        /// </summary>
        [JsonProperty("time")]
        public DateTimeOffset Time { get; init; }

        /// <summary>
        /// Contest relative time when run completed
        /// </summary>
        [JsonProperty("contest_time")]
        public TimeSpan ContestTime { get; init; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
