using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        [Authorize]
        public async Task ConnectAsync(String chatRoom)
        {
            await this.Groups.AddToGroupAsync(Context.ConnectionId, chatRoom);
            await this.Clients.Group(chatRoom).SendAsync($"{Context.ConnectionId} has left the chat.");
        }

        [Authorize]
        public async Task DisconnectAsync(String chatRoom)
        {
            await this.Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoom);
            await this.Clients.Group(chatRoom).SendAsync($"{Context.ConnectionId} has left the chat.");
        }

        [Authorize]
        public async Task SendMessage(string message, String chatRoom)
        {
            await this.Clients.Group(chatRoom).SendAsync(message);
        }
    }
}
