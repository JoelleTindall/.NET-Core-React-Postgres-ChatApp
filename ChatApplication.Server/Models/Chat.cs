//using ChatApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Server.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; } // Foreign Key pointing to AspNetUsers.Id
        public ApplicationUser User { get; set; } // Navigation property to AspNetUsers
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property to the User


    }
}