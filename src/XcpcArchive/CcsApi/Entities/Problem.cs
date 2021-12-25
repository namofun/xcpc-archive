using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// The problems to be solved in the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Problems">More detail</a>
    /// </summary>
    public class Problem : EntityBase
    {
        /// <summary>
        /// Identifier of the problem, at the WFs the directory name of the problem archive
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Label of the problem on the scoreboard, typically a single capitalized letter
        /// </summary>
        [JsonProperty("label")]
        public string Label { get; init; } = null!;

        /// <summary>
        /// Name of the problem
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Ordering of problems on the scoreboard
        /// </summary>
        [JsonProperty("ordinal")]
        public int Ordinal { get; init; }

        /// <summary>
        /// Time limit in seconds per test data set (i.e. per single run)
        /// </summary>
        [JsonProperty("time_limit")]
        public double TimeLimit { get; init; }

        /// <summary>
        /// Hexadecimal RGB value of problem color as specified in HTML hexadecimal colors, e.g. '#AC00FF' or '#fff'
        /// </summary>
        [JsonProperty("rgb")]
        public string? Rgb { get; init; }

        /// <summary>
        /// Human readable color description associated to the RGB value
        /// </summary>
        [JsonProperty("color")]
        public string? Color { get; init; }

        /// <summary>
        /// Number of test data sets
        /// </summary>
        [JsonProperty("test_data_count")]
        public int TestDataCount { get; init; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
