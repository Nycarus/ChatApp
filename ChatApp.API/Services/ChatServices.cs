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
        public List<ChatRoomUser> GetChatRooms(int userProfileId);
        public Task<bool> UserExists(int chatRoomId, int userProfileId);

        public Task<bool> AddUserToChatRoom(int chatroom, int userProfileId);

        public Task<bool> RemoveUserFromChatRoom(int chatRoomId, int userProfileId);
        public Task<ChatRoom> CreateChatRoom(string name, string description);
        public Task<ChatRoom> JoinChatRoom(int chatRoomId, int userProfileId);
        public Task<bool> LeaveChatRoom(int chatRoomId, int userProfileId);
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
                    return true;
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

        public async Task<ChatRoom> JoinChatRoom(int chatRoomId, int userProfileId)
        {
            try
            {
                var existingRoom = await _dataContext.ChatRooms.FindAsync(chatRoomId);

                if (existingRoom == null)
                {
                    _logger.LogInformation("Room does not exists.");
                    return null;
                }

                var existingUser = _dataContext.ChatRoomUsers.SingleOrDefault(o => o.ChatRoomId == chatRoomId && o.UserProfileId == userProfileId);

                if (existingUser != null)
                {
                    _logger.LogInformation("User already exists in this room.");
                    return null;
                }

                ChatRoomUser chatRoomUser = new ChatRoomUser()
                {
                    ChatRoomId = chatRoomId,
                    UserProfileId = userProfileId
                };

                var result = _dataContext.ChatRoomUsers.Add(chatRoomUser);
                _dataContext.SaveChanges();

                if (result == null)
                {
                    return null;
                }

                return existingRoom;
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task<bool> LeaveChatRoom(int chatRoomId, int userProfileId)
        {
            try
            {
                var existingUser = _dataContext.ChatRoomUsers.SingleOrDefault(o => o.ChatRoomId == chatRoomId && o.UserProfileId == userProfileId);

                if (existingUser == null)
                {
                    _logger.LogInformation("User does not exists in this chatroom.");
                    return false;
                }

                _dataContext.ChatRoomUsers.Remove(existingUser);

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
            
        }
    }
}