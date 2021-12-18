using System.Text.Json.Serialization;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Judgement types are the possible responses from the system when judging a submission.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Judgement_Types">More detail</a>
    /// </summary>
    public class JudgementType
    {
        /// <summary>
        /// Identifier of the judgement type
        /// </summary>
        /// <remarks>A 2-3 letter capitalized shorthand, see table below.</remarks>
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Name of the judgement
        /// </summary>
        /// <remarks>Might not match table below, e.g. if localised.</remarks>
        [JsonPropertyName("name")]
        public string Name { get; init; } = null!;

        /// <summary>
        /// Whether this judgement causes penalty time
        /// </summary>
        /// <remarks>Must be present if and only if <c>contest:penalty_time</c> is present.</remarks>
        [JsonPropertyName("penalty")]
        public bool Penalty { get; init; }

        /// <summary>
        /// Whether this judgement is considered correct
        /// </summary>
        [JsonPropertyName("solved")]
        public bool Solved { get; init; }
    }
}
