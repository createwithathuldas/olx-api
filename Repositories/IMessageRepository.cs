// Repositories/IMessageRepository.cs
using olx_api.Models;

namespace olx_api.Repositories
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task<IEnumerable<Message>> GetChatHistoryAsync(Guid userA, Guid userB, Guid? listingId);
        Task MarkAsReadAsync(Guid messageId);
        Task<bool> SaveChangesAsync();
    }
}