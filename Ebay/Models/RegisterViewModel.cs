using Ebay.Data;
using System.ComponentModel.DataAnnotations;

namespace Ebay.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        [UniqueEmailMobile]
        public string Email { get; set; }
        [UniqueEmailMobile]
        public string Mobile { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsActive { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class UniqueEmailMobileAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
            var model = (RegisterViewModel)validationContext.ObjectInstance;

            // Check if the email is unique
            if (dbContext.Users.Any(u => u.Email == model.Email))
            {
                return new ValidationResult("Email is already taken.");
            }

            // Check if the mobile number is unique
            if (dbContext.Users.Any(u => u.Mobile == model.Mobile))
            {
                return new ValidationResult("Mobile number is already taken.");
            }

            return ValidationResult.Success;
        }
    }


}
