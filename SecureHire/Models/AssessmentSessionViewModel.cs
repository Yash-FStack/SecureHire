using System.Collections.Generic;

namespace CheatingProofInterviewSystem.Models
{
    public class AssessmentSessionViewModel
    {
        public int AssessmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int TimeLimit { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new();
    }
}
