using ChatApp.API.Models;
using ChatApp.Data;
using ChatApp.DtoLibrary;
using ChatApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace ChatApp.API.Services
{
    public interface IChatServices
    {
        public List<ChatRoomUsers> GetChatRooms(int userProfileId);
        public Task<bool> UserExists(int chatRoomId, int userProfileId);

        public Task<bool> AddUserToChatRoom(int chatroom, int userProfileId);

        public Task<bool> RemoveUserFromChatRoom(int chatRoomId, int userProfileId);
    }
    public class ChatServices : IChatServices
    {
        private readonly ILogger<ChatServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;
        public ChatServices(DataContext dataContext, ILogger<ChatServices> logger, IConfiguration configuration) {
            _dataContext = dataContext;
            _configuration = configuration;
            _logger = logger;
        }


        public List<ChatRoomUsers> GetChatRooms(int userProfileId)
        {
            try
            {
                var chatRooms = _dataContext.UserProfiles.Include(p => p.ChatRoomUsers).Single(o => o.Id == userProfileId);
                return chatRooms.ChatRoomUsers.ToList();
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return new List<ChatRoomUsers>();
            }
        }

        public async Task<bool> UserExists(int chatRoomId, int userProfileId)
        {
            try { 
                var userExists = await _dataContext.ChatRoomUsers.AnyAsync(o =>
                    o.UserProfileId == userProfileId &&
                    o.ChatRoom.Id == chatRoomId
                    );

                return userExists;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return false;
            }
        }

        public async Task<bool> AddUserToChatRoom(int chatRoomId, int userProfileId)
        {
            try
            {

                if (await UserExists(chatRoomId, userProfileId))
                {
                    _logger.LogInformation("User Already Exists.");
                    return false;
                }

                ChatRoomUsers chatRoomUsersModel = new ChatRoomUsers();
                chatRoomUsersModel.UserProfileId = userProfileId;
                chatRoomUsersModel.ChatRoomId = chatRoomId;

                var result = await _dataContext.ChatRoomUsers.AddAsync(chatRoomUsersModel);
                _dataContext.SaveChanges();

                return result != null;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return false;
            }
        }

        public async Task<bool> RemoveUserFromChatRoom(int chatRoomId, int userProfileId)
        {
            try
            {
                var chatRoomUserToRemove = _dataContext.ChatRoomUsers.SingleOrDefault(o => o.UserProfileId == chatRoomId && o.ChatRoomId == chatRoomId);

                if (chatRoomUserToRemove == null)
                {
                    _logger.LogInformation("User does not exists in this chat room.");
                    return false;
                }

                var result = _dataContext.ChatRoomUsers.Remove(chatRoomUserToRemove);
                _dataContext.SaveChanges();

                return result != null;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return false;
            }
        }
    }
}