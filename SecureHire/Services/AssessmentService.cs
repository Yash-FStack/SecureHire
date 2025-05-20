using System.Collections.Generic;
using System.Threading.Tasks;
using CheatingProofInterviewSystem.Models;
using SecureHire.Models;

namespace CheatingProofInterviewSystem.Services
{
    public class AssessmentService : IAssessmentService
    {
        public Task<AssessmentInstructionsViewModel> GetInstructionsAsync(int assessmentId)
        {
            return Task.FromResult(new AssessmentInstructionsViewModel
            {
                AssessmentId = assessmentId,
                Title = "General Knowledge Test",
                Description = "You have 20 minutes to complete 5 questions. Do not navigate away from the screen.",
                TimeLimit = 20
            });
        }

        public Task<AssessmentSessionViewModel> StartSessionAsync(string candidateEmail, int assessmentId)
        {
            return Task.FromResult(new AssessmentSessionViewModel
            {
                AssessmentId = assessmentId,
                Title = "General Knowledge Test",
                TimeLimit = 20,
                Questions = new List<QuestionViewModel>
                {
                    new QuestionViewModel { QuestionId = 1, Text = "What is the capital of France?" },
                    new QuestionViewModel { QuestionId = 2, Text = "Who wrote 'Hamlet'?" }
                }
            });
        }

        public Task SubmitAssessmentAsync(AssessmentSessionViewModel submission)
        {
            // Store submission to DB or log (stub implementation)
            return Task.CompletedTask;
        }
    }
}
