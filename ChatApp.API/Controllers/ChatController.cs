using ChatApp.API.Models;
using ChatApp.API.Services;
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
        private readonly IHttpContextAccessor _contextAccessor;

        public ChatController(ILogger<ChatController> logger, IChatServices chatServices, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _chatServices = chatServices;
            _contextAccessor = httpContextAccessor;
        }

        [Authorize]
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            try
            {
                var user = _contextAccessor.HttpContext.User;

                if (user == null)
                {
                    throw new Exception("User does not exists.");
                }

                var userProfileClaim = user.FindFirst("UserProfileId");

                if (userProfileClaim == null)
                {
                    throw new Exception("JWT user has raised an error.");
                }
                else if (!int.TryParse(userProfileClaim.Value, out _))
                {
                    throw new Exception("JWT user is not the correct type.");
                }

                List<ChatRoomUsers> chatRooms = _chatServices.GetChatRooms(Int32.Parse(userProfileClaim.Value));

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

                return Ok(chatRooms);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return BadRequest(ex.Message);
            }
            
        }
    }
}
