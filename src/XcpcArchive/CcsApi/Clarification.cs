using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi
{
    /// <summary>
    /// Clarification message sent between teams and judges, a.k.a. clarification requests (questions from teams) and clarifications (answers from judges).
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Clarifications">More detail</a>
    /// </summary>
    public class Clarification
    {
        /// <summary>
        /// Identifier of the clarification
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Identifier of team sending this clarification request, <c>null</c> if a clarification sent by jury
        /// </summary>
        [JsonProperty("from_team_id")]
        public string? FromTeamId { get; init; }

        /// <summary>
        /// Identifier of the team receiving this reply, <c>null</c> if a reply to all teams or a request sent by a team
        /// </summary>
        [JsonProperty("to_team_id")]
        public string? ToTeamId { get; init; }

        /// <summary>
        /// Identifier of clarification this is in response to, otherwise <c>null</c>
        /// </summary>
        [JsonProperty("reply_to_id")]
        public string? ReplyToId { get; init; }

        /// <summary>
        /// Identifier of associated problem, <c>null</c> if not associated to a problem
        /// </summary>
        [JsonProperty("problem_id")]
        public string? ProblemId { get; init; }

        /// <summary>
        /// Question or reply text
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; init; } = null!;

        /// <summary>
        /// Time of the question/reply
        /// </summary>
        [JsonProperty("time")]
        public DateTimeOffset Time { get; init; }

        /// <summary>
        /// Contest time of the question/reply
        /// </summary>
        [JsonProperty("contest_time")]
        public TimeSpan ContestTime { get; init; }

        /// <summary>
        /// Extension data
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken>? ExtensionData { get; init; }
    }
}
