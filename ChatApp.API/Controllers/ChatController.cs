using AutoMapper;
using ChatApp.API.Models;
using ChatApp.API.Services;
using ChatApp.API.Utils;
using ChatApp.DtoLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> _logger;
        private readonly IChatServices _chatServices;
        private readonly IUserContext _userContext;

        public ChatController(ILogger<ChatController> logger, IChatServices chatServices, IUserContext userContext)
        {
            _logger = logger;
            _chatServices = chatServices;
            _userContext = userContext;
        }

        [Authorize]
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            try
            {
                int userProfileClaim = _userContext.getUserProfileId();

                List<ChatRoomUser> chatRooms = _chatServices.GetChatRooms(userProfileClaim);

                List<ChatRoomDTO> chatRoomDtos = new List<ChatRoomDTO>();

                foreach(var chatRoom in chatRooms)
                {
                    chatRoomDtos.Add(new ChatRoomDTO
                    {
                        Id = chatRoom.ChatRoomId,
                        Name = chatRoom.ChatRoom.Name,
                        Description = chatRoom.ChatRoom.Description
                    });
                }

                return Ok(chatRoomDtos);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize]
        [HttpPost("rooms")]
        public async Task<IActionResult> CreateChatRoom(ChatRoomDTO chatRoomDTO)
        {
            try
            {
                int userProfileId = _userContext.getUserProfileId();

                ChatRoom chatRoom = await _chatServices.CreateChatRoom(chatRoomDTO.Name, chatRoomDTO.Description);

                if (chatRoom == null)
                {
                    throw new Exception("");
                }

                await _chatServices.JoinChatRoom(chatRoom.Id, userProfileId);

                ChatRoomDTO newChatRoomDTO = new ChatRoomDTO()
                {
                    Id = chatRoom.Id,
                    Name = chatRoom.Name,
                    Description = chatRoom.Description
                };

                return Ok(newChatRoomDTO);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPost("rooms/join")]
        public async Task<IActionResult> JoinChatRoom([FromBody] int chatRoomId)
        {
            try
            {
                int userProfileId = _userContext.getUserProfileId();

                ChatRoom chatRoom = await _chatServices.JoinChatRoom(chatRoomId, userProfileId);

                if (chatRoom == null)
                {
                    throw new Exception("User unable to join chatroom.");
                }

                ChatRoomDTO newChatRoomDTO = new ChatRoomDTO()
                {
                    Id = chatRoom.Id,
                    Name = chatRoom.Name,
                    Description = chatRoom.Description
                };

                return Ok(newChatRoomDTO);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpDelete("rooms/leave")]
        public async Task<IActionResult> LeaveChatRoom([FromBody] int chatRoomId)
        {
            try
            {
                int userProfileId = _userContext.getUserProfileId();

                await _chatServices.LeaveChatRoom(chatRoomId, userProfileId);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(e.Message);
            }
        }
    }
}
