using System.ComponentModel.DataAnnotations;
using static AceJobAgency.Services.Validation;

namespace AceJobAgency.ViewModels
{
    public class Register
    {
        [Required]
        [NoSpecialCharactersAttribute]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [NoSpecialCharactersAttribute]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required, MinLength(9), MaxLength(9)]
        [NricValidation]
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
        [BirthDateValidation]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public IFormFile ResumeFile { get; set; } 
        [Required]
        public string WhoAmI { get; set; } = string.Empty;

    }
}
