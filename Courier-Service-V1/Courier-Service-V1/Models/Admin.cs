using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class Admin
    {
        public string Id { get; set; } = "AD-" + Guid.NewGuid().ToString().Substring(0, 4);
        [Required]
        public string Name { get; set; }
        [Required]
        //email validation regex
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
