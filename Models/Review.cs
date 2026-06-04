// Models/Review.cs
using System.ComponentModel.DataAnnotations;

namespace olx_api.Models
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Range(1, 5)]
        public int Rating { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // The user leaving the review
        public Guid ReviewerId { get; set; }
        public User Reviewer { get; set; }

        // The user being rated (Seller/Buyer)
        public Guid TargetUserId { get; set; }
        public User TargetUser { get; set; }
    }
}