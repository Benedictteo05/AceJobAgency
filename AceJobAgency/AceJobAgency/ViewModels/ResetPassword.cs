using System.ComponentModel.DataAnnotations;

namespace AceJobAgency.ViewModels
{
    public class ResetPassword
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Text)]
        public string Token { get; set; } = string.Empty;
    }
}
