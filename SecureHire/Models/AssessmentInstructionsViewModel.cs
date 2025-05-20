namespace CheatingProofInterviewSystem.Models
{
    public class AssessmentInstructionsViewModel
    {
        public int AssessmentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TimeLimit { get; set; } // in minutes
    }
}
