using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// The problems to be solved in the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Problems">More detail</a>
    /// </summary>
    public class Problem
    {
        /// <summary>
        /// Identifier of the problem, at the WFs the directory name of the problem archive
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Label of the problem on the scoreboard, typically a single capitalized letter
        /// </summary>
        [JsonPropertyName("label")]
        public string Label { get; init; } = null!;

        /// <summary>
        /// Name of the problem
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Ordering of problems on the scoreboard
        /// </summary>
        [JsonPropertyName("ordinal")]
        public int Ordinal { get; init; }

        /// <summary>
        /// Time limit in seconds per test data set (i.e. per single run)
        /// </summary>
        [JsonPropertyName("time_limit")]
        public double TimeLimit { get; init; }

        /// <summary>
        /// Hexadecimal RGB value of problem color as specified in HTML hexadecimal colors, e.g. '#AC00FF' or '#fff'
        /// </summary>
        [JsonPropertyName("rgb")]
        public string? Rgb { get; init; }

        /// <summary>
        /// Human readable color description associated to the RGB value
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; init; }

        /// <summary>
        /// Number of test data sets
        /// </summary>
        [JsonPropertyName("test_data_count")]
        public int TestDataCount { get; init; }
    }
}
