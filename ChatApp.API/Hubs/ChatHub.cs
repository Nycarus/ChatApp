using ChatApp.API.Models;
using ChatApp.API.Services;
using ChatApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChatApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public readonly IChatServices _chatServices;
        public readonly ILogger<ChatHub> _logger;
        public ChatHub(IChatServices chatServices, ILogger<ChatHub> logger)
        {
            _chatServices = chatServices;
            _logger = logger;
        }

        public async override Task OnConnectedAsync()
        {
            try
            {
                int userProfileId = getUserProfileId();

                _logger.LogInformation($"{userProfileId} has connected.");

                List<ChatRoomUsers> chatRoomUsers = _chatServices.GetChatRooms(userProfileId);

                foreach (ChatRoomUsers chatRoomUser in chatRoomUsers)
                {
                    await this.Groups.AddToGroupAsync(Context.ConnectionId, chatRoomUser.ChatRoomId.ToString());
                }

                await base.OnConnectedAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }
        public async Task ConnectToRoomAsync(int roomId)
        {
            try
            {
                int userProfileId = getUserProfileId();

                var result = await _chatServices.AddUserToChatRoom(roomId, userProfileId);
                await this.Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());

                await this.Clients.Group(roomId.ToString()).SendAsync("NotificationMessage", roomId.ToString(), $"{Context.ConnectionId} has left the chat room.");
            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message, e);
            }
        }
        public async Task DisconnectFromRoomAsync(int roomId)
        {
            try
            {
                int userProfileId = getUserProfileId();
                var result = await _chatServices.RemoveUserFromChatRoom(roomId, userProfileId);
                await this.Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
                await this.Clients.Group(roomId.ToString()).SendAsync("NotificationMessage", roomId.ToString(), $"{Context.ConnectionId} has left the chat room.");
            }
            catch(Exception e)
            {
                _logger.LogInformation(e.Message, e);
            }
           
        }

        public async Task SendMessage(string message, int roomId)
        {
            try
            {
                string userName = getUsername();
                await this.Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", roomId.ToString(), userName, message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }

        }

        private int getUserProfileId()
        {
            var user = Context.User;

            if (user == null)
            {
                throw new Exception("User does not exists.");
            }

            var userProfileClaim = user.FindFirst("UserProfileId");

            if (userProfileClaim == null)
            {
                throw new Exception("JWT user has raised an error.");
            }
            else if (int.TryParse(userProfileClaim.Value, out int n))
            {
                throw new Exception("JWT user is not the correct type.");
            }

            return Int32.Parse(userProfileClaim.Value);
        }

        private string getUsername()
        {
            var user = Context.User;

            if (user == null)
            {
                throw new Exception("User does not exists.");
            }

            var userNameClaim = user.FindFirst(ClaimTypes.Name);

            if (userNameClaim == null)
            {
                throw new Exception("JWT user has raised an error.");
            }

            return userNameClaim.Value;
        }
    }
}
