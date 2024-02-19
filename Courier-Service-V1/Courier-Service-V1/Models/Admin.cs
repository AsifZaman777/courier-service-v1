using System;
using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class Admin
    {
        public string Id { get; set; } = "AD-" + Guid.NewGuid().ToString().Substring(0, 4);

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
