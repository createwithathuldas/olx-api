namespace olx_api.Models
{
    public class InAppNotification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Message { get; set; }
        public string Type { get; set; } // AdApproved, MessageReceived
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}