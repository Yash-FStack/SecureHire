using System.Threading.Tasks;
using CheatingProofInterviewSystem.Models;
using SecureHire.Models;

namespace CheatingProofInterviewSystem.Services
{
    public interface IAssessmentService
    {
        Task<AssessmentInstructionsViewModel> GetInstructionsAsync(int assessmentId);
        Task<AssessmentSessionViewModel> StartSessionAsync(string candidateEmail, int assessmentId);
        Task SubmitAssessmentAsync(AssessmentSessionViewModel submission);
    }
}
