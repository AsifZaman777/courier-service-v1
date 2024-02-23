using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Courier_Service_V1.Models
{
    public class Rider
    {
        [Key]
        public string Id { get; set; } = "R-" + Guid.NewGuid().ToString().Substring(0, 4);

        [Required(ErrorMessage = "Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name should contain only alphabets.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Full address is required.")]
        public string FullAddress { get; set; }

        [Required(ErrorMessage = "District is required.")]
        public string District { get; set; }

        [Required(ErrorMessage = "Area is required.")]
        public string Area { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        public int Salary { get; set; }

        [Required(ErrorMessage = "NID is required.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Please enter a valid NID.")]
        public string NID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Contact number is required.")]
        [RegularExpression(@"^(\+88)?01[0-9]{9}$", ErrorMessage = "Please enter a valid phone number.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        public int Status { get; set; } = 1;

        public string State { get; set; } = "Available";
        

        public List<Parcel>? Parcels { get; set; }
    }
}
