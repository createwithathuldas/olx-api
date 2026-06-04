// Repositories/MessageRepository.cs
using Microsoft.EntityFrameworkCore;
using olx_api.Data;
using olx_api.Models;

namespace olx_api.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) => _context = context;

        public async Task AddMessageAsync(Message message) => await _context.Messages.AddAsync(message);

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(Guid userA, Guid userB, Guid? listingId)
        {
            var query = _context.Messages
                .Include(m => m.Sender)
                .Where(m => (m.SenderId == userA && m.ReceiverId == userB) || 
                             (m.SenderId == userB && m.ReceiverId == userA));

            if (listingId.HasValue)
                query = query.Where(m => m.ListingId == listingId.Value);

            return await query.OrderBy(m => m.SentAt).ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null) message.IsRead = true;
        }

        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
    }
}