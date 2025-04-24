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

        public async Task<ActionResult<IEnumerable<GetChatsDTO>>> GetChats()
        {
           // Console.WriteLine("GetChats endpoint hit");
            var chats = await _context.Chats
                .Include(c => c.User)
                .ThenInclude(u => u.Avatar)
                .OrderBy(c => c.CreatedAt)
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

            return Ok(chats);
        }

    }
}