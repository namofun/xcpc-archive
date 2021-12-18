using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Grouping of teams. At the World Finals these are the super regions, at regionals these are often different sites.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Groups">More detail</a>
    /// </summary>
    public class Group
    {
        /// <summary>
        /// Identifier of the group
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// External identifier from ICPC CMS
        /// </summary>
        [JsonPropertyName("icpc_id")]
        public string? IcpcId { get; init; }

        /// <summary>
        /// If group should be hidden from scoreboard
        /// </summary>
        [JsonPropertyName("hidden")]
        public bool Hidden { get; init; }

        /// <summary>
        /// Name of the group
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Type of this group via scoreboard
        /// </summary>
        [JsonPropertyName("sortorder")]
        public int SortOrder { get; init; }

        /// <summary>
        /// Type of this group via color
        /// </summary>
        [JsonPropertyName("color")]
        public string? Color { get; init; }
    }
}
