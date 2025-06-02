using CheatingProofInterviewSystem.Models;
using CheatingProofInterviewSystem.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SecureHire.Services;
using SecureHire.Services;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CheatingProofInterviewSystem.Controllers
{
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;
        [Inject] IJSRuntime JS { get; set; }
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
        public async Task<IActionResult> Take(int id = 1, bool flag=true)
        {
            if (flag)
            {
                var examSession = await _assessmentService.StartSessionAsync(User.Identity.Name, id);
                if (examSession == null)
                    return NotFound();

                return View(examSession);
            }
            else
                return null;
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

        [HttpGet]
        public async Task<int> CheckProcess()
        {
            try
            {
                var _connectionString = "server = 23.101.140.148; user id = dnh2020; password = mcn@123; database = SecureHire; TrustServerCertificate = True;";
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var email = "yash@gmail.com";
                    var sql = "SELECT ProcessesKilled FROM ProctoringEvents WHERE Email = @Email";

                    var result = await connection.QueryAsync<int>(sql, new { Email = email });

                    int processesKilled = result.FirstOrDefault(); // Safely get the first result (or 0)

                    return processesKilled;
                }

            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
