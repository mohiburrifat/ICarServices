namespace I_Car_Services.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceProviderId { get; set; }
        public int Rating { get; set; } // 1–5
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}