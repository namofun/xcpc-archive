using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// The class for job entry
    /// </summary>
    public class JobEntry : EntityBase
    {
        /// <summary>
        /// Hot identifier of the job entry
        /// </summary>
        [JsonProperty("id")]
        public string HotId { get; set; } = null!;

        /// <summary>
        /// Cold identifier of the job entry
        /// </summary>
        [JsonProperty("externalid")]
        public string ColdId { get; init; } = null!;

        /// <summary>
        /// Group partition key
        /// </summary>
        [JsonProperty("_cid")]
        public string PartitionKey { get; init; } = null!;

        /// <summary>
        /// Job creation time
        /// </summary>
        [JsonProperty("creation_time")]
        public DateTimeOffset CreationTime { get; init; }

        /// <summary>
        /// Job finished time
        /// </summary>
        [JsonProperty("finished_time")]
        public DateTimeOffset? FinishedTime { get; set; }

        /// <summary>
        /// Job status
        /// </summary>
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobStatus Status { get; set; }

        /// <summary>
        /// Job type
        /// </summary>
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public JobType Type { get; init; }

        /// <summary>
        /// Job comment
        /// </summary>
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string? Comment { get; set; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }

        /// <summary>
        /// The enum for type of job
        /// </summary>
        public enum JobType
        {
            /// <summary>
            /// Unknown job type
            /// </summary>
            Unknown,

            /// <summary>
            /// The job is a post-contest zip upload
            /// </summary>
            FullZipUpload,

            /// <summary>
            /// The job is a real-time stream uploading
            /// </summary>
            RealTimeStream,
        }

        /// <summary>
        /// The enum for status of job
        /// </summary>
        public enum JobStatus
        {
            /// <summary>
            /// Unknown job status
            /// </summary>
            Unknown,

            /// <summary>
            /// Job is pending
            /// </summary>
            Pending,

            /// <summary>
            /// Job has been finished
            /// </summary>
            Complete,

            /// <summary>
            /// Job failed to execute
            /// </summary>
            Failed,
        }
    }
}
