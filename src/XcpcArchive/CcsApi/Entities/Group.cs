using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi.Entities
{
    /// <summary>
    /// Grouping of teams. At the World Finals these are the super regions, at regionals these are often different sites.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Groups">More detail</a>
    /// </summary>
    public class Group : EntityBase
    {
        /// <summary>
        /// Identifier of the group
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Name of the group
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Name of the group
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string? Type { get; init; }

        /// <summary>
        /// If group should be hidden from scoreboard
        /// </summary>
        [JsonProperty("hidden")]
        public bool Hidden { get; init; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }

        /// <summary>
        /// Unknown group place holder
        /// </summary>
        public static Group Unknown { get; } = new Group() { Hidden = true };
    }
}
