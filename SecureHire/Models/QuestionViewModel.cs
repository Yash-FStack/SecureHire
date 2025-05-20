namespace CheatingProofInterviewSystem.Models
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; } // Used for binding candidate's response
    }
}
