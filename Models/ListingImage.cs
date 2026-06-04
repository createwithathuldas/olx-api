using System.ComponentModel.DataAnnotations;

namespace olx_api.Models
{
    public class ListingImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }

        public Guid ListingId { get; set; }
        public Listing Listing { get; set; }
    }
}