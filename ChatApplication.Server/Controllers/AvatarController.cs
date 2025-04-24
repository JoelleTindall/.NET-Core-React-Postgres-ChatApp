using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AvatarController : ControllerBase
    {
        private readonly ChatAppContext _context;


        public AvatarController(ChatAppContext context)
        {
            _context = context;
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
            var user = await _context.Users.FindAsync(setAvatarDTO.UserId);
            if (user == null) return Unauthorized();
            var avatar = await _context.Avatars.FindAsync(setAvatarDTO.AvatarId);
            if (avatar == null) return NotFound("Avatar not found");

            user.AvatarId = avatar.Id;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

    }
}
