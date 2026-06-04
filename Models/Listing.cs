using System.ComponentModel.DataAnnotations;

namespace olx_api.Models
{
    public class Listing
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(150)]
        public string Title { get; set; }
        [Required, MaxLength(2000)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsNegotiable { get; set; }

        [Required]
        public string State { get; set; } // e.g., Kerala, Tamil Nadu
        [Required]
        public string City { get; set; }

        public string Status { get; set; } = "Active"; // Active, Sold, Moderation
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsFeatured { get; set; } = false;
        public DateTime LastBoostedAt { get; set; } = DateTime.UtcNow;

        public string CustomAttributesJson { get; set; }

        // Foreign Keys & Navigation
        public Guid UserId { get; set; }
        public User User { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<ListingImage> Images { get; set; }
        public ICollection<Favorite> FavoritedBy { get; set; }
    }
}