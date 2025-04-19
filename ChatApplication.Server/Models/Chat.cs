namespace ChatApplication.Server.Models
{
    public class Chat
    {
    public int Id { get; set; }
    public User User { get; set; } // Navigation property
    public int UserId { get; set; } // Change UserId to string
    public string UserName { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }

    }
}