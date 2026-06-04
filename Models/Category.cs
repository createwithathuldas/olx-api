using System.ComponentModel.DataAnnotations;

namespace olx_api.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public string IconUrl { get; set; }
        
        // Self-referencing relationship for sub-categories (e.g., Vehicles -> Cars)
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; }
        
        public ICollection<Listing> Listings { get; set; }
    }
}