using Microsoft.AspNetCore.Identity;

namespace ChatApplication.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? AvatarId { get; set; }  // FK to Avatar table
        // Add navigation property to Avatar
        public virtual Avatar Avatar { get; set; }

        public ApplicationUser()
        {
            // Set default AvatarId to "1" (or whatever default ID you want)
            AvatarId = 1;
        }

    }
}