namespace I_Car_Services.Models
{
    public class Balance
    {
        public int Id { get; set; }

        public int UserId { get; set; }  // ID of the user this balance belongs to

        public decimal Amount { get; set; } // The balance amount
    }
}
