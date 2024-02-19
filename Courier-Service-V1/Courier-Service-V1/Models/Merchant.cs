using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courier_Service_V1.Models
{
    public class Merchant
    {
        [Key]
        public string Id { get; set; } = "M-" + Guid.NewGuid().ToString().Substring(0, 4); 
        [Required]
        //error message for name
        public string CompanyName { get; set; }
        [Required]
        
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only alphabets")]
        public string Name { get; set; }
        [Required]
        public string FullAddress { get; set; }
        [Required]
        public string CompanyAddress { get; set; }
        [Required]
        public string District { get; set; }
        [Required]
        public string Area { get; set; }
        
        [Required]
        
        [RegularExpression(@"^(\+88)?01[0-9]{9}$", ErrorMessage = "Please enter a valid phone number")]
        public string ContactNumber { get; set; }

        //Facebook URL validation regex
        [RegularExpression(@"(http(s)?:\/\/)?(www\.)?facebook.com\/[a-zA-Z0-9(\.\?)?]", ErrorMessage = "Please enter a valid Facebook URL")]
        public string? FacebookUrl { get; set; }
        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", ErrorMessage = "Please enter a valid URL")]
        public string? Website { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }
        [Required]
        //email validation regex
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid NID")]
        public int NID { get; set; }
        [Required]
        public string TradeLicense { get; set; }
        [Required]
        public string Tin { get; set; }
        public List<Parcel>? Parcels { get; set; }

    }
}
