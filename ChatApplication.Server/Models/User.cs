
namespace ChatApplication.Server.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
        public required string PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }

        public bool IsAdmin { get; set; }
        public bool? IsBanned { get; set; }

        // Navigation properties
        public int AvatarId { get; set; }
        public Avatar Avatar { get; set; }
        public required ICollection<Chat> Chats { get; set; }
    }
}