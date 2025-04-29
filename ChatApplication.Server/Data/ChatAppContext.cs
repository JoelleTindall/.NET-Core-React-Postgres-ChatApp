using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using ChatApplication.Server.Models;
using ChatApplication.Server.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ChatApplication.Server.Data
{
    public class ChatAppContext : DbContext
    {

        public ChatAppContext(DbContextOptions<ChatAppContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Chat> Chats { get; set; } = default!;
        public DbSet<Avatar> Avatars { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Avatar>().HasData(
            new Avatar { Id = 1, FilePath = "img/cat.png" },
            new Avatar { Id = 2, FilePath = "img/egg.png" },
            new Avatar { Id = 3, FilePath = "img/mad.png" });

            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Admin"));
            var passwordSalt = hmac.Key;
            builder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserName = "Admin",
                    PasswordHash = Convert.ToBase64String(passwordHash),
                    PasswordSalt = Convert.ToBase64String(passwordSalt),
                    CreatedAt = DateTime.UtcNow,
                    IsAdmin = true,
                    AvatarId = 1, // Default avatar ID
                    Chats = new List<Chat>() // Initialize Chats as an empty list});
                });

            // Define the relationship between ApplicationUser and Avatar
            builder.Entity<User>()
                .HasOne(u => u.Avatar)  // A user has one avatar
                .WithMany()              // One avatar can be used by multiple users
                .HasForeignKey(u => u.AvatarId) // Foreign key to Avatar table
            .IsRequired(false);

            builder.Entity<Chat>()
                .HasOne(c => c.User) // A chat belongs to one user
                .WithMany(u => u.Chats) // One user can have many chats
                .HasForeignKey(c => c.UserId); // Foreign key to User table


        }


    }
}