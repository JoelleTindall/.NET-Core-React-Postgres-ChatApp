using Microsoft.EntityFrameworkCore;
using ChatApplication.Server.Models;

namespace ChatApplication.Server.Data
{
    public class ChatAppContext : DbContext
    {
        // Required constructor for dependency injection
        public ChatAppContext(DbContextOptions<ChatAppContext> options)
            : base(options)
        {
        }

        // Optional parameterless constructor for migrations
        public ChatAppContext()
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Avatar> Avatars { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure your entity relationships here
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        }
    }
}