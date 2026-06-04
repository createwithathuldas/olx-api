namespace olx_api.Models
{
    public class Favorite
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid ListingId { get; set; }
        public Listing Listing { get; set; }
        
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}