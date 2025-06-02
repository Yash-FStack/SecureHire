using CheatingProofInterviewSystem.Models;
using SecureHire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureHire.Services;
using System.Threading.Tasks;
using CheatingProofInterviewSystem.Services;

namespace CheatingProofInterviewSystem.Controllers
{
    [Authorize(Roles = "Candidate")]
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        // GET: /Assessment/Instructions/5
        [HttpGet]
        public async Task<IActionResult> Instructions(int id)
        {
            var instructions = await _assessmentService.GetInstructionsAsync(id);
            if (instructions == null)
                return NotFound();

            return View(instructions);
        }

        // GET: /Assessment/Take/5
        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var examSession = await _assessmentService.StartSessionAsync(User.Identity.Name, id);
            if (examSession == null)
                return NotFound();

            return View(examSession);
        }
        

        // POST: /Assessment/Submit
        [HttpPost]
        public async Task<IActionResult> Submit(AssessmentSessionViewModel submission)
        {
            if (!ModelState.IsValid)
                return View("Take", submission);

            await _assessmentService.SubmitAssessmentAsync(submission);

            TempData["Message"] = "Assessment submitted successfully.";
            return RedirectToAction("Dashboard", "Candidate");
        }



        [HttpGet]
        public IActionResult PreTake()
        {
            
            return View("PreCheck");
        }
    }
}
