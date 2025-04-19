namespace ChatApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? AvatarId { get; set; } = 1; // FK to Avatar table
                                            // Add navigation property to Avatar
        public virtual Avatar? Avatar { get; set; }


    }
}
////using Microsoft.AspNetCore.Identity;

//namespace ChatApplication.Server.Models
//{
//    public class ApplicationUser : IdentityUser
//    {
//public int? AvatarId { get; set; }  // FK to Avatar table
//                                    // Add navigation property to Avatar
//public virtual Avatar Avatar { get; set; }

//public ApplicationUser()
//{
//    // Set default AvatarId to "1" (or whatever default ID you want)
//    AvatarId = 1;
//}

//    }
//}
