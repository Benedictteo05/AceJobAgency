using System.ComponentModel.DataAnnotations;

namespace AceJobAgency.ViewModels
{
    public class Register
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required, MinLength(8), MaxLength(8)]
        public string NRIC { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string ResumeFile { get; set; } = string.Empty;
        [Required]
        public string WhoAmI { get; set; } = string.Empty;
    }
}
