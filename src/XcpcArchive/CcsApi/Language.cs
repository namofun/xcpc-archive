using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Languages that are available for submission at the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Languages">More detail</a>
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Identifier of the language from table below
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Name of the language
        /// </summary>
        /// <remarks>Might not match table below, e.g. if localised.</remarks>
        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// File extensions of the language
        /// </summary>
        [JsonPropertyName("extensions")]
        public string[]? Extensions { get; init; }

        /// <summary>
        /// If allow judge solutions in this language
        /// </summary>
        [JsonPropertyName("allow_judge")]
        public bool AllowJudge { get; init; }

        /// <summary>
        /// Time factor for running solutions
        /// </summary>
        [JsonPropertyName("time_factor")]
        public double TimeFactor { get; init; }
    }
}
