using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "ID is required.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Old password is required.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmation password is required.")]
        public string ConfirmPassword { get; set; }
    }
}
