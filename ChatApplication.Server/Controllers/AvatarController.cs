using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChatApplication.Server.Controllers
{
    
    [ApiController]
    [Route("avatar")]
    public class AvatarController : ControllerBase
    {
        private readonly ChatAppContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public AvatarController(ChatAppContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> SetAvatar([FromBody] int avatarId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var avatar = await _context.Avatars.FindAsync(avatarId);
            if (avatar == null) return NotFound("Avatar not found");

            user.AvatarId = avatar.Id;
            await _userManager.UpdateAsync(user);

            return Ok(new { success = true });
        }

        //// POST api/<AvatarContoller>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<AvatarContoller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AvatarContoller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
