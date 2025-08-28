using System;

namespace I_Car_Services.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceProviderId { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
    }
}
