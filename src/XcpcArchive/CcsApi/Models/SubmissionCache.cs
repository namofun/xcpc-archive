using Newtonsoft.Json;
using System;

namespace XcpcArchive.CcsApi.Models
{
    public class SubmissionCache
    {
        [JsonProperty("s")]
        public string SubmissionId { get; init; } = null!;

        [JsonProperty("j")]
        public string JudgementId { get; init; } = null!;

        [JsonProperty("v")]
        public string? JudgementTypeId { get; init; }

        [JsonProperty("p")]
        public string ProblemId { get; init; } = null!;

        [JsonProperty("t")]
        public string TeamId { get; init; } = null!;

        [JsonProperty("z")]
        public int ContestTime { get; init; }

        public SubmissionCache()
        {
            // Empty constructor for JSON deserializing
        }

        public SubmissionCache(Entities.Submission s, Entities.Judgement j)
        {
            if (s.Id != j.SubmissionId)
            {
                throw new InvalidOperationException("Mismatch submission and judgement entity join");
            }

            this.SubmissionId = s.Id;
            this.JudgementId = j.Id;
            this.JudgementTypeId = j.JudgementTypeId;
            this.ProblemId = s.ProblemId;
            this.TeamId = s.TeamId;
            this.ContestTime = (int)s.ContestTime.TotalSeconds;
        }
    }
}
