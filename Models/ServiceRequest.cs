namespace I_Car_Services.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceProviderId { get; set; }
        public string ServiceType { get; set; }
        public string Status { get; set; } // "Pending", "In Progress", "Completed"
        public string Address { get; set; }
        public DateTime RequestTime { get; set; }
        public bool PaidOnline { get; set; }
    }
}