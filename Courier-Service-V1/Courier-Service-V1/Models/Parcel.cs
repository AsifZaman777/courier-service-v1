using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courier_Service_V1.Models
{
    public class Parcel
    {
        [Key]
        public string Id { get; set; } = "P-" + Guid.NewGuid().ToString().Substring(0, 4);

        [Required(ErrorMessage = "Receiver name is required")]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Receiver address is required")]
        public string ReceiverAddress { get; set; }

        [Required(ErrorMessage = "Receiver contact number is required")]
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Invalid contact number")]
        public string ReceiverContactNumber { get; set; }

        [Required(ErrorMessage = "Receiver email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string ReceiverEmail { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product weight is required")]
        public decimal ProductWeight { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        public int ProductPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be at least 1")]
        public int ProductQuantity { get; set; }

        [Required(ErrorMessage = "Delivery charge is required")]
        public int DeliveryCharge { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "PickupRequested";

        
        [ForeignKey("MerchantId")]
        public string? MerchantId { get; set; }
        public Merchant? Merchant { get; set; }
        [ForeignKey("RiderId")]
        public string? RiderId { get; set; }
        public Rider? Rider { get; set; }
    }
}
