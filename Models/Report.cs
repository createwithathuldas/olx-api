namespace olx_api.Models;

public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Reason { get; set; } // Spam, Fake, Scam
        public string Status { get; set; } = "Pending"; // Pending, Reviewed, Dismissed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid ReporterId { get; set; }
        public User Reporter { get; set; }
        
        public Guid ReportedListingId { get; set; }
        public Listing ReportedListing { get; set; }
    }