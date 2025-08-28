namespace I_Car_Services.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ServiceProviderId { get; set; }
    }
}