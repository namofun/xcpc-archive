using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi.Entities
{
    /// <summary>
    /// Teams can be associated with organizations which will have some associated information, e.g. a logo. Typically organizations will be universities.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Organizations">More detail</a>
    /// </summary>
    public class Organization : EntityBase
    {
        /// <summary>
        /// Identifier of the organization
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Short display name of the organization
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Full organization name if too long for normal display purposes.
        /// </summary>
        [JsonProperty("formal_name")]
        public string FormalName { get; init; } = null!;

        /// <summary>
        /// ISO 3-letter code of the organization's country
        /// </summary>
        [JsonProperty("country", NullValueHandling = NullValueHandling.Ignore)]
        public string? Country { get; init; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
