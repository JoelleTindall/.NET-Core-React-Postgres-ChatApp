namespace ChatApplication.Server.Models.DTO
{
    public class GetChatsDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarFilePath { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
