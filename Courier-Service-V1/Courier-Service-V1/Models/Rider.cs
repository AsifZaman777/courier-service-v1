using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courier_Service_V1.Models
{
    public class Rider
    {
        [Key]
        public string Id { get; set; } = "R-" + Guid.NewGuid().ToString().Substring(0, 4);

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only alphabets")]
        public string Name { get; set; }

        [Required]
        public string FullAddress { get; set; }

        [Required]
        public string District { get; set; }

        [Required]
        public string Area { get; set; }

        [Required]
        public int Salary { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid NID")]
        public int NID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        [RegularExpression(@"^(\+88)?01[0-9]{9}$", ErrorMessage = "Please enter a valid phone number")]
        public string ContactNumber { get; set; }

        [Required]
        
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
       
        public string Password { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        public int Status { get; set; } = 1;

        public string State { get; set; } = "Available";
        public List<Parcel>? Parcels { get; set; }
    }

}
