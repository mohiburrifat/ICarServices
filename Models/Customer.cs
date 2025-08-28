namespace I_Car_Services.Models
{
    public class Customer : User
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        
        public bool Customer_IsApproved { get; set; }
    }
}
