using System.Reflection.Metadata.Ecma335;
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Controllers
{
    [ApiController]
    [Route("chat")]
    public class ChatController : ControllerBase
    {
        private readonly ChatAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public ChatController(ChatAppContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // List to hold chat messages
        public List<Chat> Chats { get; set; } = new List<Chat>();

        public string CurrentUser { get; set; }

       // private static string User = _userManager.GetUserId(User);

        [HttpGet(Name = "GetChats")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            var currentUserId = _userManager.GetUserId(User);  // Optional, in case you need to use it

            var chats = await _context.Chats
                .Include(c => c.User)
                .ThenInclude(u => u.Avatar)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return Ok(chats);
        }
        //public async Task OnGetAsync()
        //{
        //    CurrentUser = _userManager.GetUserId(User);  // Get the current user ID

        //    // Get the list of chat messages from the database
        //    Chats = await _context.Chats
        //        .Include(c => c.User)  // Include the User object (User and Avatar are linked)
        //        .ThenInclude(u => u.Avatar)  // Include the Avatar object
        //        .OrderBy(c => c.CreatedAt)  // Order by creation date
        //        .ToListAsync();


        //}


    }
}
