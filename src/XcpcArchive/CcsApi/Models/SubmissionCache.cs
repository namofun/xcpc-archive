using Newtonsoft.Json;

namespace XcpcArchive.CcsApi.Models
{
    public class SubmissionCache
    {
        [JsonProperty("s")]
        public string SubmissionId { get; init; } = null!;

        [JsonProperty("j")]
        public string? JudgementId { get; init; } = null!;

        [JsonProperty("v")]
        public string? JudgementTypeId { get; init; }

        [JsonProperty("l")]
        public string LanguageId { get; init; } = null!;

        [JsonProperty("p")]
        public string ProblemId { get; init; } = null!;

        [JsonProperty("t")]
        public string TeamId { get; init; } = null!;

        [JsonProperty("z")]
        public int ContestTime { get; init; }
    }
}
