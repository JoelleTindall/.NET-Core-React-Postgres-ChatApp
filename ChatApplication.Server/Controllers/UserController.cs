using Microsoft.AspNetCore.Mvc;
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Models.DTO;

using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq.Expressions;


namespace ChatApplication.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ChatAppContext _context;

        public UserController(ChatAppContext context)
        {
            _context = context;

        }
        public List<User> Users { get; set; } = new List<User>();

        [HttpGet(Name = "GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {

            var users = await _context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("{userid}/admin")]
        public async Task<IActionResult> MarkAdmin(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {

                user.IsAdmin = !user.IsAdmin;
                await _context.SaveChangesAsync();
                Console.WriteLine("User " + user.UserName + " admin: " + (user.IsAdmin).ToString());
                //var users = await _context.Users.ToListAsync();
                return Ok(user);
            }
            else
            {
                return NotFound("User not found.");
            }
        }

    }
}

