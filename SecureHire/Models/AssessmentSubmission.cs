using System.Collections.Generic;

namespace CheatingProofInterviewSystem.Models
{
    public class AssessmentSubmission
    {
        public int AssessmentId { get; set; }
        public string CandidateEmail { get; set; }
        public List<AnswerSubmission> Answers { get; set; }
    }

    public class AnswerSubmission
    {
        public int QuestionId { get; set; }
        public string AnswerText { get; set; }
    }
}
