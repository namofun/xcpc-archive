﻿using Newtonsoft.Json;
using System;

namespace XcpcArchive.CcsApi.Models
{
    /// <summary>
    /// Current state of the contest, specifying whether it's running, the scoreboard is frozen or results are final.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Contest_state">More detail</a>
    /// </summary>
    public class State
    {
        /// <summary>
        /// Time when the contest actually started, or <c>null</c> if the contest has not started yet.
        /// </summary>
        /// <remarks>When set, this time must be equal to the contest <c>start_time</c>.</remarks>
        [JsonProperty("started")]
        public DateTimeOffset? Started { get; init; }

        /// <summary>
        /// Time when the scoreboard was frozen, or <c>null</c> if the scoreboard has not been frozen.
        /// </summary>
        /// <remarks>Required iff <c>scoreboard_freeze_duration</c> is present in the contest endpoint.</remarks>
        [JsonProperty("frozen")]
        public DateTimeOffset? Frozen { get; init; }

        /// <summary>
        /// Time when the contest ended, or <c>null</c> if the contest has not ended.
        /// </summary>
        /// <remarks>Must not be set if started is <c>null</c>.</remarks>
        [JsonProperty("ended")]
        public DateTimeOffset? Ended { get; init; }

        /// <summary>
        /// Time when the scoreboard was thawed (that is, unfrozen again), or <c>null</c> if the scoreboard has not been thawed.
        /// </summary>
        /// <remarks>Required iff <c>scoreboard_freeze_duration</c> is present in the contest endpoint. Must not be set if frozen is <c>null</c>.</remarks>
        [JsonProperty("thawed")]
        public DateTimeOffset? Thawed { get; init; }

        /// <summary>
        /// Time when the results were finalized, or <c>null</c> if results have not been finalized.
        /// </summary>
        /// <remarks>Must not be set if ended is <c>null</c>.</remarks>
        [JsonProperty("finalized")]
        public DateTimeOffset? Finalized { get; init; }

        /// <summary>
        /// Time after last update to the contest occurred, or <c>null</c> if more updates are still to come.
        /// </summary>
        /// <remarks>Setting this to non-<c>null</c> must be the very last change in the contest.</remarks>
        [JsonProperty("end_of_updates")]
        public DateTimeOffset? EndOfUpdates { get; init; }
    }
}
