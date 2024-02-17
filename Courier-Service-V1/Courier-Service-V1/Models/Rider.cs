using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class Rider
    {
        [Key]
        
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FullAddress { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Area { get; set; }
        [Required]
        public string Salary { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ImageUrl { get; set; }

        public int Status { get; set; } = 1;

        public string State { get; set; } = "Available";
    }
}
