using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi.Entities
{
    /// <summary>
    /// Teams competing in the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Teams">More detail</a>
    /// </summary>
    public class Team : EntityBase
    {
        /// <summary>
        /// Identifier of the team
        /// </summary>
        /// <remarks>Usable as a label, at WFs normally the team seat number.</remarks>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// External identifier from ICPC CMS
        /// </summary>
        [JsonProperty("icpc_id")]
        public string? IcpcId { get; init; }

        /// <summary>
        /// Name of the team
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Display name of the team
        /// </summary>
        /// <remarks>If not set, a client should revert to using the name instead.</remarks>
        [JsonProperty("display_name")]
        public string? DisplayName { get; init; }

        /// <summary>
        /// Identifier of the organization (e.g. university or other entity) that this team is affiliated to
        /// </summary>
        [JsonProperty("organization_id")]
        public string? OrganizationId { get; init; }

        /// <summary>
        /// Identifiers of the group(s) this team is part of (at ICPC WFs these are the super-regions)
        /// </summary>
        /// <remarks>No meaning must be implied or inferred from the order of IDs. The array may be empty.</remarks>
        [JsonProperty("group_ids")]
        public string[] GroupIds { get; init; } = Array.Empty<string>();

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
