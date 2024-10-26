using System.ComponentModel.DataAnnotations;

namespace FuCommunityWebUtility
{
    public class ValidationHelper
    {
        public static ValidationResult ValidateDateOfBirth(DateTime? dateOfBirth, ValidationContext context)
        {
            if (dateOfBirth.HasValue)
            {
                if (dateOfBirth.Value > DateTime.Now)
                {
                    return new ValidationResult("Date of birth cannot be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
