using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Controllers
{


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
        public async Task<ActionResult<IEnumerable<GetChatsDTO>>> GetChats([FromQuery] DateTime? before = null, [FromQuery] int pageSize = 10)
        {
            // gets the chats from the database
            var query = _context.Chats
                .Include(c => c.User)
                .ThenInclude(u => u.Avatar)
                .Where(c => (c.IsDeleted == false || c.IsDeleted == null)
                && (c.User.IsBanned == false || c.User.IsBanned == null))  //return only chats that are not deleted from users not banned (including null for backwards compatibility)
                .OrderByDescending(c => c.CreatedAt)  // newest first

                .AsQueryable();

            //compares the last datetime already returned to the next (previous) chat in the database
            if (before.HasValue)
            {
                query = query.Where(c => c.CreatedAt < before.Value);
            }

            //returns max 10 chats at a time
            var chats = await query
                .Take(pageSize)
                .Select(c => new
                {
                    id = c.Id,
                    message = c.Message,
                    createdAt = c.CreatedAt,
                    user = new
                    {
                        id = c.User.Id.ToString(),
                        userName = c.User.UserName,
                        avatar = new
                        {
                            id = c.User.Avatar.Id,
                            filePath = c.User.Avatar.FilePath
                        }
                    }
                })
                .ToListAsync();

            return Ok(chats.OrderBy(c => c.createdAt)); // return oldest to newest
        }


    }
}