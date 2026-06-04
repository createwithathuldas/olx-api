// Repositories/IListingRepository.cs
using olx_api.Models;

namespace olx_api.Repositories
{
    public interface IListingRepository
    {
        Task<IEnumerable<Listing>> GetAllAsync(string? search, int? categoryId, string? city, int page, int pageSize);
        Task<Listing?> GetByIdAsync(Guid id);
        Task AddAsync(Listing listing);
        Task UpdateAsync(Listing listing);
        Task DeleteAsync(Listing listing);
        Task<bool> SaveChangesAsync();
    }
}