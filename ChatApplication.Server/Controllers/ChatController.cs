using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Controllers
{

    public class NewChatDto
    {
        public string Message { get; set; }
    }

    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ChatAppContext _context;
        //private readonly UserManager<ApplicationUser> _userManager;


        public ChatController(ChatAppContext context)
        {
            _context = context;
          //  _userManager = userManager;
        }

        // List to hold chat messages
        public List<Chat> Chats { get; set; } = new List<Chat>();

        public string CurrentUser { get; set; }

       // private static string User = _userManager.GetUserId(User);

        [HttpGet(Name = "GetChats")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
          //  var currentUserId = _userManager.GetUserId(User);  // Optional, in case you need to use it

            var chats = await _context.Chats
                .Include(c => c.User)
                .ThenInclude(u => u.Avatar)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(chats);
        }
        [HttpPost]
        public async Task<ActionResult<Chat>> PostChat([FromBody] NewChatDto incomingChat)
        {
           // var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user_Id= User.FindFirstValue(ClaimTypes.NameIdentifier);
           int userId=Int32.Parse(user_Id);
            //  var userId = _userManager.GetUserId(User);
            var user = await _context.Users
                .Include(u => u.Avatar)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return Unauthorized();

            var newChat = new Chat
            {
                Message = incomingChat.Message,
                CreatedAt = DateTime.UtcNow,
                User = user
            };

            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();

            var result = await _context.Chats
                .Include(c => c.User)
                .ThenInclude(u => u.Avatar)
                .FirstOrDefaultAsync(c => c.Id == newChat.Id);

            return Ok(result);

        }



    }
}
