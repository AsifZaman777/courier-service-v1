using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class Merchant
    {
        [Key]
        public string Id { get; set; } = "M-" + Guid.NewGuid().ToString().Substring(0, 4); 
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FullAddress { get; set; }
        [Required]
        public string CompanyAddress { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Area { get; set; }
        //branch(optional for now)
        [Required]
        public string ContactNumber { get; set; }
       
        public string? FacebookUrl { get; set; }

        public string? Website { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string NID { get; set; }
        [Required]
        public string TradeLicense { get; set; }
        [Required]
        public string Tin { get; set; }

    }
}
