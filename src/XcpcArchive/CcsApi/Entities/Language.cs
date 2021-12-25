using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Languages that are available for submission at the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Languages">More detail</a>
    /// </summary>
    public class Language : EntityBase
    {
        /// <summary>
        /// Identifier of the language from table below
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Name of the language
        /// </summary>
        /// <remarks>Might not match table below, e.g. if localised.</remarks>
        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
