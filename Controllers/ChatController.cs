using Microsoft.AspNetCore.Mvc;
using olx_api.DTOs;
using olx_api.Repositories;
using System.Security.Claims;

namespace olx_api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IMessageRepository _messageRepo;

        public ChatController(IMessageRepository messageRepo)
        {
            _messageRepo = messageRepo;
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<MessageResponseDto>>> GetHistory([FromQuery] Guid otherUserId, [FromQuery] Guid? listingId)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (otherUserId == userId.Value)
                return BadRequest("Chat history requires two distinct users.");

            var messages = await _messageRepo.GetChatHistoryAsync(userId.Value, otherUserId, listingId);
            return Ok(messages.Select(m => new MessageResponseDto(
                m.Id,
                m.Content,
                m.SentAt,
                m.IsRead,
                m.SenderId,
                m.Sender.FullName,
                m.ReceiverId
            )));
        }

        private Guid? GetCurrentUserId()
        {
            var value =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue("sub") ??
                User.FindFirstValue("nameid");

            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }
}
