using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ChatApplication.Server.Models; 
using ChatApplication.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Controllers
{
    [ApiController]
    [Route("account")]
    public class UserController : ControllerBase
    {
        private readonly ChatAppContext _context;

        public UserController(ChatAppContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName && u.Password == Hash(request.Password));

            if (user == null)
                return Unauthorized(new { error = "Invalid credentials" });

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new { message = "Logged in" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logged out" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == request.UserName))
                return BadRequest(new { error = "User already exists" });
            var newUser = new User
            {
                UserName = request.UserName,
                Password = Hash(request.Password),
                AvatarId = 1 // Default avatar
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User registered" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                username = User.Identity?.Name
            });
        }

        // Optional: helper method for password hashing (you can use BCrypt instead)
        private string Hash(string password)
        {
            // Just a placeholder - do NOT use this in prod
            return password; // replace with secure hash!
        }
    }
}