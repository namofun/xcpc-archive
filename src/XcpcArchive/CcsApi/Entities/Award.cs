using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi.Entities
{
    /// <summary>
    /// Awards such as medals, first to solve, etc.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Awards">More detail</a>
    /// </summary>
    public class Award : EntityBase
    {
        /// <summary>
        /// Identifier of the award
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Award citation, e.g. "Gold medal winner"
        /// </summary>
        [JsonProperty("citation")]
        public string Citation { get; init; } = null!;

        /// <summary>
        /// JSON array of team ids receiving this award
        /// </summary>
        /// <remarks>No meaning must be implied or inferred from the order of IDs. The array may be empty.</remarks>
        [JsonProperty("team_ids")]
        public string[] TeamIds { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
