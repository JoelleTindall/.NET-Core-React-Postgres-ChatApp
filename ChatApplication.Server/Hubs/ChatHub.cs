using ChatApplication.Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatApplication.Server.Data; // Your DbContext

namespace ChatApplication.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatAppContext _context;

        public ChatHub(ChatAppContext context)
        {
            _context = context;
        }

        // This method sends a message and includes the user's avatar URL
        public async Task SendMessage(string userId, string message)
        {
            // Get the user associated with the userId from the database
            var user = await _context.Users
                .Include(u => u.Avatar)  // Include Avatar navigation property
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return; // If no user is found, do not proceed
            }

            // Retrieve the user's avatar URL or use a default if not set
            var avatarUrl = user.Avatar?.FilePath ?? "img/egg.png"; // Default avatar if none exists

            // Create a new Chat message
            var chatMessage = new Chat
            {
                UserId = userId,
                UserName = user.UserName,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };
            Console.WriteLine($"Saving chat message: {chatMessage.Message}");
            // Save the chat message to the database
            _context.Chats.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", userId, user.UserName, message, avatarUrl, chatMessage.CreatedAt);
        }
    }
}