using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace XcpcArchive.CcsApi.Models
{
    /// <summary>
    /// Scoreboard of the contest.
    /// <a href="https://clics.ecs.baylor.edu/index.php?title=Contest_API_2020#Scoreboard">More detail</a>
    /// </summary>
    /// <remarks>Since this is generated data, only the GET method is allowed here, irrespective of role.</remarks>
    public class Scoreboard
    {
        /// <summary>
        /// Identifier of the event after which this scoreboard was generated.
        /// </summary>
        /// <remarks>This must be identical to the argument <c>after_event_id</c>, if specified.</remarks>
        [JsonProperty("event_id")]
        public string EventId { get; set; } = "archived0";

        /// <summary>
        /// Time contained in the associated event
        /// </summary>
        /// <remarks>Implementation defined if the event has no associated time.</remarks>
        [JsonProperty("time")]
        public DateTimeOffset Time { get; set; }

        /// <summary>
        /// Contest time contained in the associated event
        /// </summary>
        /// <remarks>Implementation defined if the event has no associated contest time.</remarks>
        [JsonProperty("contest_time")]
        public TimeSpan ContestTime { get; set; }

        /// <summary>
        /// Identical data as returned by the contest state endpoint
        /// </summary>
        /// <remarks>This is provided here for ease of use and to guarantee the data is synchronized.</remarks>
        [JsonProperty("state")]
        public State State { get; set; } = null!;

        /// <summary>
        /// A list of rows of team with their associated scores
        /// </summary>
        [JsonProperty("rows")]
        public IEnumerable<Row> Rows { get; set; } = null!;

        /// <summary>
        /// The class for score object.
        /// </summary>
        public class Score : IEquatable<Score>
        {
            /// <summary>
            /// Number of problems solved by the team
            /// </summary>
            /// <remarks>Required iff <c>contest:scoreboard_type</c> is <c>pass-fail</c></remarks>
            [JsonProperty("num_solved", NullValueHandling = NullValueHandling.Ignore)]
            public int? NumSolved { get; set; }

            /// <summary>
            /// Total penalty time accrued by the team
            /// </summary>
            /// <remarks>Required iff <c>contest:scoreboard_type</c> is <c>pass-fail</c></remarks>
            [JsonProperty("total_time", NullValueHandling = NullValueHandling.Ignore)]
            public int? TotalTime { get; set; }

            /// <summary>
            /// Total score of problems by the team
            /// </summary>
            /// <remarks>Required iff <c>contest:scoreboard_type</c> is <c>score</c></remarks>
            [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
            public double? PartialScore { get; set; }

            /// <summary>
            /// Time of last score improvement used for tiebreaking purposes
            /// </summary>
            [JsonProperty("time")]
            public int LastAccepted { get; set; }

            /// <inheritdoc />
            public bool Equals(Score? other)
            {
                if (other == null) return false;
                return this.NumSolved == other.NumSolved
                    && this.TotalTime == other.TotalTime
                    && this.PartialScore == other.PartialScore
                    && this.LastAccepted == other.LastAccepted;
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
            {
                return Equals(obj as Score);
            }

            /// <inheritdoc />
            public override int GetHashCode()
            {
                return HashCode.Combine(NumSolved, TotalTime, PartialScore, LastAccepted);
            }
        }

        /// <summary>
        /// The class for problem object.
        /// </summary>
        public class Problem
        {
            /// <summary>
            /// Identifier of the problem
            /// </summary>
            [JsonProperty("problem_id")]
            public string ProblemId { get; set; } = null!;

            /// <summary>
            /// Label of the problem
            /// </summary>
            [JsonProperty("label")]
            public string Label { get; set; } = null!;

            /// <summary>
            /// Number of judged submissions (up to and including the first correct one)
            /// </summary>
            [JsonProperty("num_judged")]
            public int NumJudged { get; set; }

            /// <summary>
            /// Number of pending submissions (either queued or due to freeze)
            /// </summary>
            [JsonProperty("num_pending")]
            public int NumPending { get; set; }

            /// <summary>
            /// Whether the team solved this problem
            /// </summary>
            /// <remarks>Required iff <c>contest:scoreboard_type</c> is <c>pass-fail</c></remarks>
            [JsonProperty("solved", NullValueHandling = NullValueHandling.Ignore)]
            public bool? Solved { get; set; }

            /// <summary>
            /// The score of the last submission from team
            /// </summary>
            /// <remarks>
            /// Required iff <c>contest:scoreboard_type</c> is <c>score</c> and <c>solved</c> is missing.
            /// If missing or null defaults to 100 if solved is true and 0 if solved is false
            /// </remarks>
            [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
            public double? Score { get; set; }

            /// <summary>
            /// Minutes into the contest when this problem was solved by the team
            /// </summary>
            /// <remarks>Required iff <c>solved=true</c></remarks>
            [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
            public int? Time { get; set; }

            /// <summary>
            /// Whether the team is first to solve this problem
            /// </summary>
            [JsonProperty("first_to_solve", NullValueHandling = NullValueHandling.Ignore)]
            public bool? FirstToSolve { get; set; }
        }

        /// <summary>
        /// The class for a row of scoreboard.
        /// </summary>
        public class Row
        {
            /// <summary>
            /// Rank of this team, 1-based and duplicate in case of ties
            /// </summary>
            [JsonProperty("rank")]
            public int Rank { get; set; }

            /// <summary>
            /// Identifier of the team
            /// </summary>
            [JsonProperty("team_id")]
            public string TeamId { get; set; } = null!;

            /// <summary>
            /// JSON object as specified in the rows below (for possible extension to other scoring methods)
            /// </summary>
            [JsonProperty("score")]
            public Score Score { get; set; } = new();

            /// <summary>
            /// JSON array of problems with scoring data, see below for the specification of each element
            /// </summary>
            [JsonProperty("problems")]
            public Problem[] Problems { get; set; } = Array.Empty<Problem>();
        }
    }
}
