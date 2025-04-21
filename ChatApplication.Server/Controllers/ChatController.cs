using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Controllers
{
    public class NewChatDto
    {
        public string Message { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatAppContext _context;
        public ChatController(ChatAppContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetChats")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            var chats = await _context.Chats
                .Include(c => c.User)
                .ThenInclude(u => u.Avatar)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(chats);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Chat>> PostChat([FromBody] NewChatDto incomingChat)
        {
            // Hardcoded demo user - REPLACE THIS WITH YOUR ACTUAL USER AUTH LOGIC
            var userId = 1; // Example: Get from session/token instead of Identity
            var user = await _context.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return Unauthorized();

            var newChat = new Chat
            {
                Message = incomingChat.Message,
                CreatedAt = DateTime.UtcNow,
                UserId = userId, // Set foreign key directly
                //UserName = user.UserName
            };

            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();

            // Return the saved chat with user/avatar data
            var result = await _context.Chats
                .Include(c => c.User)
                .ThenInclude(u => u.Avatar)
                .FirstOrDefaultAsync(c => c.Id == newChat.Id);

            return Ok(result);
        }
    }
}