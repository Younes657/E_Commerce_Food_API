namespace E_Commerce_Food_API.Models.DTO
{
    public class OrderHeaderUpdateDTO
    {
        public int Id { get; set; }
        public string PickupName { get; set; }
        public string PickupPhoneNumber { get; set; }
        public string PickupEmail { get; set; }

        //public string StripePaymentIntentID { get; set; }
        public string Status { get; set; }
    }
}
