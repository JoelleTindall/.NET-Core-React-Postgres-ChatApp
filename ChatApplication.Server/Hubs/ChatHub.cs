using ChatApplication.Server.Data;
using ChatApplication.Server.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApplication.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatAppContext _context;

        public ChatHub(ChatAppContext context)
        {
            _context = context;
        }

        public async Task SendMessage(int userId, string message) // Changed from string userId to int
        {
            // Get the user from the plain Users table (no Identity)
            var user = await _context.Users
                .Include(u => u.Avatar)  // Include Avatar if needed
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return; // User not found
            }

            // Get avatar URL (or default)
            var avatarUrl = user.Avatar?.FilePath ?? "img/egg.png";

            // Create and save chat message
            var chatMessage = new Chat
            {
                UserId = userId,
                UserName = user.UserName,
                Message = message,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chats.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Broadcast the message
            await Clients.All.SendAsync("ReceiveMessage", userId, user.UserName, message, avatarUrl, chatMessage.CreatedAt);
        }
    }
}