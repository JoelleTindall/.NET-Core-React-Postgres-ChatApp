//using ChatApplication.Models;
//using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Server.Models
{
    public class Chat
    {
        public int Id { get; set; }
        //public string UserName { get; set; }
        public  int UserId { get; set; } // Foreign Key pointing to AspNetUsers.Id
        public  User? User { get; set; } // Navigation property to AspNetUsers
        public required string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property to the User


    }
}