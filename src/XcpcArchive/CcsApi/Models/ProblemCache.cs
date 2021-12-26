using Newtonsoft.Json;

namespace XcpcArchive.CcsApi.Models
{
    public class ProblemCache
    {
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        [JsonProperty("label")]
        public string Label { get; init; } = null!;

        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        [JsonProperty("ordinal")]
        public int Ordinal { get; init; }

        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public string? Rgb { get; init; }
    }
}
