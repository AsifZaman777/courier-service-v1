using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class ResetPassword
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
