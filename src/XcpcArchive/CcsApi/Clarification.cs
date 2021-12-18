using System;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("id")]
        public string Id { get; init; } = null!;

        /// <summary>
        /// Identifier of team sending this clarification request, <c>null</c> if a clarification sent by jury
        /// </summary>
        [JsonPropertyName("from_team_id")]
        public string? FromTeamId { get; init; }

        /// <summary>
        /// Identifier of the team receiving this reply, <c>null</c> if a reply to all teams or a request sent by a team
        /// </summary>
        [JsonPropertyName("to_team_id")]
        public string? ToTeamId { get; init; }

        /// <summary>
        /// Identifier of clarification this is in response to, otherwise <c>null</c>
        /// </summary>
        [JsonPropertyName("reply_to_id")]
        public string? ReplyToId { get; init; }

        /// <summary>
        /// Identifier of associated problem, <c>null</c> if not associated to a problem
        /// </summary>
        [JsonPropertyName("problem_id")]
        public string? ProblemId { get; init; }

        /// <summary>
        /// Question or reply text
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; init; } = null!;

        /// <summary>
        /// Time of the question/reply
        /// </summary>
        [JsonPropertyName("time")]
        public DateTimeOffset Time { get; init; }

        /// <summary>
        /// Contest time of the question/reply
        /// </summary>
        [JsonPropertyName("contest_time")]
        public TimeSpan ContestTime { get; init; }
    }
}
