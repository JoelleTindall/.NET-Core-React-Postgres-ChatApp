namespace ChatApplication.Server.Models.DTO
{
    public class LoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}