using System.Reflection.Emit;
using ChatApplication.Server.Models;
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
            new Avatar { Id = 3, FilePath = "img/mad.png" }

);
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