using System.ComponentModel.DataAnnotations;

namespace Ebay.Models.Admin
{
    public class admin
    {
        public int Id { get; set; }
        public string admin_name { get; set; }
        [Required]
        [RegularExpression("([a-zA-Z0-9 .&'-]+)", ErrorMessage = "Invalid Username")]
        public string Username { get; set; }
        [Required]
   
        public string Password { get; set; }
    }
}
