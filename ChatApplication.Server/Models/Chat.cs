namespace ChatApplication.Server.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key
        public  User User { get; set; } // Navigation property
        public  string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool? IsDeleted { get; set; }

    }
}