using ChatApp.API.Models;
using ChatApp.API.Services;
using ChatApp.API.Utils;
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
        private readonly IChatServices _chatServices;
        private readonly ILogger<ChatHub> _logger;
        private readonly IUserContext _userContext;
        public ChatHub(IChatServices chatServices, ILogger<ChatHub> logger, IUserContext userContext)
        {
            _chatServices = chatServices;
            _logger = logger;
            _userContext = userContext;
        }

        public async override Task OnConnectedAsync()
        {
            try
            {
                int userProfileId = _userContext.getUserProfileId();

                _logger.LogInformation($"{userProfileId} has connected.");

                List<ChatRoomUser> chatRoomUsers = _chatServices.GetChatRooms(userProfileId);

                foreach (ChatRoomUser chatRoomUser in chatRoomUsers)
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
                int userProfileId = _userContext.getUserProfileId();

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
                int userProfileId = _userContext.getUserProfileId();
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
                string userName = _userContext.getUsername();
                await this.Clients.Group(roomId.ToString()).SendAsync("ReceiveMessage", roomId.ToString(), userName, message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
            }

        }
    }
}
