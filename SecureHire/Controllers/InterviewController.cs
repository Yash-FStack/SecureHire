using Microsoft.AspNetCore.Mvc;

namespace SecureHire.Controllers
{
    public class InterviewController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
