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
