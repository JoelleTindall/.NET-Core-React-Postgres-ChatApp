// Controllers/RegisterController.cs
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ChatAppContext _context;

        public RegisterController(ChatAppContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Username) ||
                string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return BadRequest("Username and password are required");
            }
            if (await _context.Users.AnyAsync(u => u.UserName == registerDto.Username))
            {
                return Conflict("Username already exists");
            }

            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            var passwordSalt = hmac.Key;
            var user = new User
            {
                UserName = registerDto.Username,
                PasswordHash = Convert.ToBase64String(passwordHash),
                PasswordSalt = Convert.ToBase64String(passwordSalt),
                CreatedAt = DateTime.UtcNow,
                Chats = new List<Chat>() // Initialize Chats as an empty list
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                Message = "Registration successful"
            });
        }

        private bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            var hashBytes = Convert.FromBase64String(storedHash);

            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != hashBytes[i]) return false;
            }
            return true;
        }
    }
}