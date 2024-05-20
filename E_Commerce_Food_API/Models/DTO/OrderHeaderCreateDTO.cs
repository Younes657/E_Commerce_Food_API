using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Food_API.Models.DTO
{
    public class OrderHeaderCreateDTO
    {
        [Required]
        public string PickupName { get; set; }
        [Required]
        public string PickupPhoneNumber { get; set; }
        [Required]
        public string PickupEmail { get; set; }

        public string UserId { get; set; }
        public double OrderTotal { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }
        public string StripePaymentIntentID { get; set; }
        //public IEnumerable<OrderDetailsCreateDto> OrderDetailsDTO { get; set; }
        public List<OrderDetailsCreateDto> OrderDetailsDTO { get; set; } = [];

    }
}
