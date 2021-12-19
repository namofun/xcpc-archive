using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Submissions, a.k.a. attempts to solve problems in the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Submissions">More detail</a>
    /// </summary>
    public class Submission
    {
        /// <summary>
        /// Identifier of the submission
        /// </summary>
        /// <remarks>Usable as a label, typically a low incrementing number</remarks>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Identifier of the language submitted for
        /// </summary>
        [JsonProperty("language_id")]
        public string LanguageId { get; init; } = null!;

        /// <summary>
        /// Identifier of the problem submitted for
        /// </summary>
        [JsonProperty("problem_id")]
        public string ProblemId { get; init; } = null!;

        /// <summary>
        /// Identifier of the team that made the submission
        /// </summary>
        [JsonProperty("team_id")]
        public string TeamId { get; init; } = null!;

        /// <summary>
        /// Timestamp of when the submission was made
        /// </summary>
        [JsonProperty("time")]
        public DateTimeOffset Time { get; init; }

        /// <summary>
        /// Contest relative time when the submission was made
        /// </summary>
        [JsonProperty("contest_time")]
        public TimeSpan ContestTime { get; init; }

        /// <summary>
        /// Code entry point for specific languages
        /// </summary>
        [JsonProperty("entry_point")]
        public string? EntryPoint { get; init; }

        /// <summary>
        /// Submission files, contained at the root of the archive
        /// </summary>
        /// <remarks>Only allowed mime type is <c>application/zip</c>.</remarks>
        [JsonProperty("files")]
        public FileMeta[] Files { get; set; } = Array.Empty<FileMeta>();

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }

        /// <summary>
        /// Zip bytes of submission files, in which should be only one
        /// </summary>
        [JsonProperty("_filezip", NullValueHandling = NullValueHandling.Ignore)]
        public byte[]? FileContent { get; set; }

        /// <summary>
        /// Metadata for file archive.
        /// </summary>
        public sealed class FileMeta
        {
            /// <summary>
            /// The file href
            /// </summary>
            [JsonProperty("href")]
            public string Href { get; init; } = null!;

            /// <summary>
            /// The file mime type
            /// </summary>
            [JsonProperty("mime")]
            public string Mime { get; init; } = null!;
        }
    }
}
