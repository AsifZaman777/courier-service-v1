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

        

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? ReceiverEmail { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product weight is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Product weight can't be below 0")]
        public decimal ProductWeight { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        [Range(1,int.MaxValue,ErrorMessage ="Product price can't be below 0")]
        public int ProductPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Product quantity must be at least 1")]

        public int? ProductQuantity { get; set; }

        [Required(ErrorMessage = "Delivery charge is required")]
        [Range(0,1000,ErrorMessage ="Delivery charge range is 0 to 1000")]
        public int DeliveryCharge { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = "Pickup Request";
        public DateTime? PickupRequestDate { get; set; } = DateTime.Now;
        public DateTime? DispatchDate  { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentInHand { get; set; }
        [Required(ErrorMessage = "Pickup location is required")]
        public string PickupLocation { get; set; }
        public string DeliveryType { get; set; }
        public int TotalPrice { get; set; }
        
        [ForeignKey("MerchantId")]
        public string? MerchantId { get; set; }
        public Merchant? Merchant { get; set; }
        [ForeignKey("RiderId")]
        public string? RiderId { get; set; }
        public Rider? Rider { get; set; }
    }
}
