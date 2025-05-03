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
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ChatAppContext _context;
        private readonly IConfiguration _config;

        public LoginController(ChatAppContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Both fields are required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
            if (user != null)
            {

                if (VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                {
                    if (user.IsBanned == true)
                    {
                        return Unauthorized("User is banned");
                    }

                    string userId = user.Id.ToString();
                    string isAdmin = user.IsAdmin.ToString();

                    var token = GenerateJwtToken(loginDto.Username, userId, isAdmin);
                    return Ok(new
                    {
                        token
                    });
                }
                else
                {
                    return Unauthorized("Invalid username or password");
                }
            }
            else
            {
                return Unauthorized("Invalid username or password");
            }

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

        private string GenerateJwtToken(string username, string id, string isAdmin)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim("userId", id),
            new Claim("isAdmin", (isAdmin).ToLower()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
