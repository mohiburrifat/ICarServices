namespace I_Car_Services.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string PaymentMethod { get; set; } 
    }
}