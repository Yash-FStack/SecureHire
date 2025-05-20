using Microsoft.AspNetCore.Identity;

namespace CheatingProofInterviewSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extend with additional fields if needed
        public bool IsIdentityVerified { get; set; }
        public string WebcamPhotoBase64 { get; set; }
        public string UploadedIDPath { get; set; }
    }
}
