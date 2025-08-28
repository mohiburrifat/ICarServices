namespace I_Car_Services.Models
{
    public class MyServiceProvider : User
    {
        public string BusinessName { get; set; }
        public string Description { get; set; }
        public bool IsApproved { get; set; } = false;
    }
}