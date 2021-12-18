using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Teams can be associated with organizations which will have some associated information, e.g. a logo. Typically organizations will be universities.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Organizations">More detail</a>
    /// </summary>
    public class Organization
    {
        /// <summary>
        /// Identifier of the organization
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// External identifier from ICPC CMS
        /// </summary>
        [JsonPropertyName("icpc_id")]
        public string? IcpcId { get; init; }

        /// <summary>
        /// Short display name of the organization
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Short display name of the organization
        /// </summary>
        [JsonPropertyName("shortname")]
        public string ShortName { get; init; } = null!;

        /// <summary>
        /// Full organization name if too long for normal display purposes.
        /// </summary>
        [JsonPropertyName("formal_name")]
        public string FormalName { get; init; } = null!;

        /// <summary>
        /// ISO 3-letter code of the organization's country
        /// </summary>
        [JsonPropertyName("country")]
        public string? Country { get; init; }
    }
}
