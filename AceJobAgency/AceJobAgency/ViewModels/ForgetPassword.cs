using System.ComponentModel.DataAnnotations;

namespace AceJobAgency.ViewModels
{
    public class ForgetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
