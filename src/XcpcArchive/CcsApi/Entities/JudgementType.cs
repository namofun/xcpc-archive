using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi.Entities
{
    /// <summary>
    /// Judgement types are the possible responses from the system when judging a submission.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Judgement_Types">More detail</a>
    /// </summary>
    public class JudgementType : EntityBase
    {
        /// <summary>
        /// Identifier of the judgement type
        /// </summary>
        /// <remarks>A 2-3 letter capitalized shorthand, see table below.</remarks>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Name of the judgement
        /// </summary>
        /// <remarks>Might not match table below, e.g. if localised.</remarks>
        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Whether this judgement causes penalty time
        /// </summary>
        /// <remarks>Must be present if and only if <c>contest:penalty_time</c> is present.</remarks>
        [JsonProperty("penalty")]
        public bool Penalty { get; init; }

        /// <summary>
        /// Whether this judgement is considered correct
        /// </summary>
        [JsonProperty("solved")]
        public bool Solved { get; init; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
