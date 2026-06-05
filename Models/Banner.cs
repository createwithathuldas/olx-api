public class Banner
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string PlacementType { get; set; } // Home, Promotional
        public bool IsActive { get; set; } = true;
    }