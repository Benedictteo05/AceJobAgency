using Microsoft.AspNetCore.Identity;

namespace AceJobAgency.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string NRIC { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ResumeFile { get; set; }
        public string WhoAmI { get; set; }
    }
}
