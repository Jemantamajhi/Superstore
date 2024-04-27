using System.ComponentModel.DataAnnotations;

namespace Ebay.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
