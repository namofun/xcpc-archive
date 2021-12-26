using Newtonsoft.Json;
using System;

namespace XcpcArchive.CcsApi.Models
{
    public class TeamCache
    {
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        [JsonProperty("name")]
        public string Name { get; init; } = null!;

        [JsonProperty("organization")]
        public string? Organization { get; init; }

        [JsonProperty("groups")]
        public string[] Groups { get; init; } = Array.Empty<string>();
    }
}
