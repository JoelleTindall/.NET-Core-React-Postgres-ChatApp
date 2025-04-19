using Microsoft.AspNetCore.Mvc;
using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using ChatApplication.Server.Models.DTO;

using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;


namespace ChatApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly ChatAppContext _context;

        public LoginController(ChatAppContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.Username) ||
                string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest("Both fields are required.");
            }

            //using var hmac = new HMACSHA512();
            //var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            //var passwordSalt = hmac.Key;

            //string UserName = loginDto.Username;
            //var PasswordHash = Convert.ToBase64String(passwordHash);
            //var PasswordSalt = Convert.ToBase64String(passwordSalt);
            //(u => u.UserName == loginDto.Username && u.PasswordHash == PasswordHash && u.PasswordSalt == PasswordSalt))
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

            //if (await _context.Users.AnyAsync(u => u.UserName == loginDto.Username))
           // {
                if (VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return Ok(new
                    {
                        Id = user.Id,
                        Username = user.UserName,
                        Message = "Login successful"
                    });
                }
                else
                {
                    return Unauthorized("Invalid username or password");
                }

                
        
            //using var hmac = new HMACSHA512();
            //var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            //var passwordSalt = hmac.Key;

            //var UserName = loginDto.Username;
            //var PasswordHash = Convert.ToBase64String(passwordHash);
            //var PasswordSalt = Convert.ToBase64String(passwordSalt);


            //_context.Users.Add(user);
            //await _context.SaveChangesAsync();

            //return Ok(new
            //{
            //    Id = user.Id,
            //    Username = user.UserName,
            //    Message = "Registration successful"
            //});

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
