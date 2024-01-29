using Microsoft.AspNetCore.Identity;

namespace AceJobAgency.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string NRIC { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string ResumeFile { get; set; } = string.Empty;
        public string WhoAmI { get; set; } = string.Empty;
        public string SessionId {  get; set; } = string.Empty;
        public string SecondOldPassword { get; set; } = string.Empty;
        public DateTime LastPasswordChange {  get; set; } = DateTime.Now;
    }
}
