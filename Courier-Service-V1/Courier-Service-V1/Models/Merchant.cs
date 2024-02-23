using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class Merchant
    {
        [Key]
        public string Id { get; set; } = "M-" + Guid.NewGuid().ToString().Substring(0, 4);

        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only alphabets.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Full address is required.")]
        public string FullAddress { get; set; }

        [Required(ErrorMessage = "Company address is required.")]
        public string CompanyAddress { get; set; }

        [Required(ErrorMessage = "District is required.")]
        public string District { get; set; }

        [Required(ErrorMessage = "Area is required.")]
        public string Area { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^(\+88)?01[0-9]{9}$", ErrorMessage = "Please enter a valid phone number.")]
        public string ContactNumber { get; set; }

        //regx for facebook url
        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", ErrorMessage = "Please enter a valid URL.")]
        public string? FacebookUrl { get; set; }

        [RegularExpression(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$", ErrorMessage = "Please enter a valid URL.")]
        public string? Website { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        //regx for NID
        [Required(ErrorMessage = "NID is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid NID.")]

        public string NID { get; set; }

        [Required(ErrorMessage = "Trade license is required.")]
        public string TradeLicense { get; set; }

        [Required(ErrorMessage = "TIN is required.")]
        public string Tin { get; set; }

        public List<Parcel>? Parcels { get; set; }
    }
}
