// Repositories/ListingRepository.cs
using Microsoft.EntityFrameworkCore;
using olx_api.Data;
using olx_api.Models;

namespace olx_api.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly ApplicationDbContext _context;

        public ListingRepository(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Listing>> GetAllAsync(string? search, int? categoryId, string? city, int page, int pageSize)
        {
            var query = _context.Listings
                .Include(l => l.Images)
                .Include(l => l.Category)
                .Include(l => l.User)
                .Where(l => l.Status == "Active");

            if (!string.IsNullOrEmpty(search))
                query = query.Where(l => l.Title.Contains(search) || l.Description.Contains(search));

            if (categoryId.HasValue)
                query = query.Where(l => l.CategoryId == categoryId.Value);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(l => l.City.Equals(city, StringComparison.OrdinalIgnoreCase));

            // Update inside Repositories/ListingRepository.cs -> GetAllAsync
            return await query
                .OrderByDescending(l => l.IsFeatured)      // 1st Priority: Featured ads pin to top
                .ThenByDescending(l => l.LastBoostedAt)   // 2nd Priority: Most recently bumped/created ads
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Listing?> GetByIdAsync(Guid id) =>
            await _context.Listings
                .Include(l => l.Images)
                .Include(l => l.Category)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

        public async Task AddAsync(Listing listing) => await _context.Listings.AddAsync(listing);
        public async Task UpdateAsync(Listing listing) => await Task.Run(() => _context.Listings.Update(listing));
        public async Task DeleteAsync(Listing listing) => await Task.Run(() => _context.Listings.Remove(listing));
        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}