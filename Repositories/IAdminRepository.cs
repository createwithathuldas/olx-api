using olx_api.Models;

namespace olx_api.Repositories
{
    public interface IAdminRepository
    {
        Task<int> GetTotalUsersAsync();
        Task<int> GetActiveListingsCountAsync();
        Task<IEnumerable<Report>> GetPendingReportsAsync();
        // ... CMS endpoints for Banners and Static Pages
    }
}