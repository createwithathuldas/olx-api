using System.ComponentModel.DataAnnotations;

namespace olx_api.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Content { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        public Guid SenderId { get; set; }
        public User Sender { get; set; }

        public Guid ReceiverId { get; set; }
        public User Receiver { get; set; }

        // Optional: Link chat to a specific ad
        public Guid? ListingId { get; set; }
        public Listing Listing { get; set; }
    }
}