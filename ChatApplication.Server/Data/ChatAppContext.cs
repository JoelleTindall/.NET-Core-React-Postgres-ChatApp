using System.Reflection.Emit;
using ChatApplication.Server.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace ChatApplication.Server.Data
{
    public class ChatAppContext : IdentityDbContext<ApplicationUser>  // Use ApplicationUser here
    {

        public ChatAppContext(DbContextOptions<ChatAppContext> options)
            : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; } = default!;
        public DbSet<Avatar> Avatars { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
           // 8da2ec45 - a26a - 4643 - ac5c - b0514f2eb803
            base.OnModelCreating(builder);
            builder.Entity<Avatar>().HasData(
            new Avatar { Id = 1, FilePath = "img/cat.png" },
            new Avatar { Id = 2, FilePath = "img/egg.png" },
            new Avatar { Id = 3, FilePath = "img/mad.png" }

);
            builder.Entity<Chat>().HasData(
                new Chat { Id =1, 
                    UserId = "8da2ec45-a26a-4643-ac5c-b0514f2eb803",
                UserName="Test",
                Message="This is a test message",
                CreatedAt= DateTime.UtcNow}
                );
            // Define the relationship between ApplicationUser and Avatar
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Avatar)  // A user has one avatar
                .WithMany()              // One avatar can be used by multiple users
                .HasForeignKey(u => u.AvatarId) // Foreign key to Avatar table
            .IsRequired(false);
        }


    }
}