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
        public Task<ChatRoom> GetChatRoom(int chatRoomId);
        public List<ChatRoomUser> GetChatRooms(int userProfileId);
        public Task<bool> UserExists(int chatRoomId, int userProfileId);

        public Task<bool> AddUserToChatRoom(int chatroom, int userProfileId);

        public Task<bool> RemoveUserFromChatRoom(int chatRoomId, int userProfileId);
        public Task<ChatRoom> CreateChatRoom(string name, string description);
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

        public async Task<ChatRoom> GetChatRoom(int chatRoomId)
        {
            try
            {
                ChatRoom chatRoom = await _dataContext.ChatRooms.FindAsync(chatRoomId);
                return chatRoom;
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return null;
            }
        }


        public List<ChatRoomUser> GetChatRooms(int userProfileId)
        {
            try
            {
                var chatRooms = _dataContext.ChatRoomUsers.Include(p => p.ChatRoom).Where(o => o.UserProfileId == userProfileId);
                return chatRooms.ToList();
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return new List<ChatRoomUser>();
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

                if (_dataContext.ChatRooms.Find(chatRoomId) == null)
                {
                    _logger.LogInformation("Server Does Not Exists.");
                    return false;
                }

                ChatRoomUser chatRoomUsersModel = new ChatRoomUser();
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
                var chatRoomUserToRemove = _dataContext.ChatRoomUsers.SingleOrDefault(o => o.UserProfileId == userProfileId && o.ChatRoomId == chatRoomId);

                if (chatRoomUserToRemove == null)
                {
                    _logger.LogInformation("User does not exists in this chat room.");
                    return true;
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

        public async Task<ChatRoom> CreateChatRoom(string name, string description)
        {
            try
            {
                ChatRoom chatRoom = new ChatRoom()
                {
                    Name = name,
                    Description = description
                };

                await _dataContext.ChatRooms.AddAsync(chatRoom);
                _dataContext.SaveChanges();

                return chatRoom;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null; 
            }
        }
    }
}