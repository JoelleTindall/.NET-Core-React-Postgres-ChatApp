using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Models.DTO;

//using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApplication.Server.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AvatarController : ControllerBase
    {
        private readonly ChatAppContext _context;
        private readonly ILogger _logger;
        //private readonly UserManager<Users> _userManager;


        public AvatarController(ChatAppContext context, ILogger<AvatarController> logger)
        {
            _context = context;
            _logger = logger;

            //  _userManager = userManager;
        }

        public List<Avatar> Avatars { get; set; } = new List<Avatar>();
        public int CurrentAvatar { get; set; }
   

        [HttpGet(Name = "GetAvatars")]
        public async Task<ActionResult<IEnumerable<Avatar>>> GetAvatars()
        {

            var avatars = await _context.Avatars.ToListAsync();

            return Ok(avatars);
        }

        [HttpPost("set")]
        public async Task<IActionResult> SetAvatar([FromBody] SetAvatarDTO setAvatarDTO)
        {
            _logger.LogInformation("SetAvatar endpoint hit");
            var user = await _context.Users.FindAsync(setAvatarDTO.UserId);
            if (user == null) return Unauthorized();
            _logger.LogInformation("attempting update user {UserId} with new avatar {AvatarId}", setAvatarDTO.UserId, setAvatarDTO.AvatarId);
            var avatar = await _context.Avatars.FindAsync(setAvatarDTO.AvatarId);
            if (avatar == null) return NotFound("Avatar not found");

            user.AvatarId = avatar.Id;
            _logger.LogInformation("Updating user {UserId} with new avatar {AvatarId}", user.Id, avatar.Id);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

    }
}
