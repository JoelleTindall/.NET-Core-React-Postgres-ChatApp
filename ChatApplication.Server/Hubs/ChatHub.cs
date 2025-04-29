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

        public async Task MarkDeleted(int chatId)
        {
            // Find the chat message by ID
            var chatMessage = await _context.Chats.FindAsync(chatId);
            if (chatMessage != null)
            {
                // Mark the chat message as deleted
                chatMessage.IsDeleted = true;
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("RemovedChat", chatMessage.Id);

            }
        }

        public async Task MarkBanned(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                if (user.IsBanned == true)
                {
                    Console.WriteLine($" {userId} is already banned.");
                    return; // If the user is already banned, do not proceed
                }

                user.IsBanned = true;
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("UserBanned", (user.Id).ToString());
            } else
            {
                Console.WriteLine("user not found.");
            }
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
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false // default false
            };

            // Save the chat message to the database
            _context.Chats.Add(chatMessage);
            await _context.SaveChangesAsync();

            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", user.Id.ToString(), user.UserName, message, avatarUrl, chatMessage.CreatedAt);
        }
    }
}