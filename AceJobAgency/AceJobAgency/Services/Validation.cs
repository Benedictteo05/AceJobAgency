using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AceJobAgency.Services
{
    public class Validation
    {
        public class NoSpecialCharactersAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var input = (string)value;

                if (!string.IsNullOrEmpty(input))
                {
                    if (!Regex.IsMatch(input, "^[a-zA-Z ]+$"))
                    {
                        var displayName = validationContext.DisplayName;
                        return new ValidationResult($"Special characters are not allowed in {displayName} field.");
                    }
                }

                return ValidationResult.Success;
            }
        }

        public class NricValidationAttribute : ValidationAttribute
        {
            private readonly string pattern = "^[STFGstfg]\\d{7}[A-Za-z]$";

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var nric = (string)value;

                if (string.IsNullOrEmpty(nric) || !Regex.IsMatch(nric, pattern))
                {
                    return new ValidationResult("Invalid NRIC format.");
                }

                return ValidationResult.Success;
            }
        }

        public class BirthDateValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var birthDate = (DateTime)value;

                if (birthDate >= DateTime.Today)
                {
                    return new ValidationResult("BirthDate must be before today.");
                }

                return ValidationResult.Success;
            }
        }
    }
}
