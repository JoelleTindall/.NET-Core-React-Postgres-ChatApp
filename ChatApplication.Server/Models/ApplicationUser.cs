

namespace ChatApplication.Server.Models
{
    public class ApplicationUser
    {
        public int? AvatarId { get; set; }
        public virtual Avatar Avatar { get; set; }

        public ApplicationUser()
        {
            // Set default AvatarId to "1" (or whatever default ID you want)
            AvatarId = 1;
        }

    }c
}