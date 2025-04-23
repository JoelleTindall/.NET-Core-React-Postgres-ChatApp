using ChatApplication.Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatApplication.Server.Data; 

namespace ChatApplication.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatAppContext _context;
      
        public ChatHub(ChatAppContext context)
        {
            _context = context;
         
        }

        // Sends chat message and adds it to the database
        public async Task SendMessage(int userId, string message)
        {
           
            // Get the user associated with the userId from the database
            var user = await _context.Users
                .Include(u => u.Avatar)  // Include Avatar navigation property
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return; // If no user is found, do not proceed
            }

            // get the avatar filepath from the user
            var avatarUrl = user.Avatar.FilePath;

            // create a new chat message
            var chatMessage = new Chat
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };
           
            // Save the chat message to the database
            _context.Chats.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user.Id.ToString(), user.UserName, message, avatarUrl, chatMessage.CreatedAt);
        }
    }
}