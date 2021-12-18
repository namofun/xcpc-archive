using System;
using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Awards such as medals, first to solve, etc.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Awards">More detail</a>
    /// </summary>
    public class Award
    {
        /// <summary>
        /// Identifier of the award
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Award citation, e.g. "Gold medal winner"
        /// </summary>
        [JsonPropertyName("citation")]
        public string Citation { get; init; } = null!;

        /// <summary>
        /// JSON array of team ids receiving this award
        /// </summary>
        /// <remarks>No meaning must be implied or inferred from the order of IDs. The array may be empty.</remarks>
        [JsonPropertyName("team_ids")]
        public string[] TeamIds { get; init; } = Array.Empty<string>();
    }
}
