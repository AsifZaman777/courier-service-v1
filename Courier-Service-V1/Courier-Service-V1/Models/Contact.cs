using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class Contact
    {
        [Key]
        public string Id { get; set; } = "C-" + Guid.NewGuid().ToString().Substring(0, 4);
        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only alphabets.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^(\+88)?01[0-9]{9}$", ErrorMessage = "Please enter a valid phone number.")]
        public string ContactNumber { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        public string? Subject { get; set; }
        [Required(ErrorMessage = "Message is required.")]
        public string Message { get; set; }
    }
}
