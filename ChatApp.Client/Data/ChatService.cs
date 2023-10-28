using ChatApp.DtoLibrary;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml.Linq;

namespace ChatApp.Client.Data
{
    public interface IChatService
    {
        public Task<List<ChatRoom>> GetRoomList();
        public Task<ChatRoom> CreateNewRoom(string name, string description);
        public Task<ChatRoom> JoinChatRoom(int roomId);
    }
    public class ChatService : IChatService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ChatService> _logger;
        private readonly IConfiguration _configuration;

        public ChatService(IHttpClientFactory httpClientFactory, ILogger<ChatService> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<List<ChatRoom>> GetRoomList()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("ChatAppApi");
            var roomResponse = await httpClient.GetAsync($"{_configuration.GetSection("API").Value}/api/chat/rooms");

            if (!roomResponse.IsSuccessStatusCode)
            {
                throw new Exception(roomResponse.StatusCode.ToString());
            }

            List<ChatRoomDTO> rooms = JsonConvert.DeserializeObject<List<ChatRoomDTO>>(await roomResponse.Content.ReadAsStringAsync());
            List<ChatRoom> result = new List<ChatRoom>();

            foreach (ChatRoomDTO room in rooms)
            {
                result.Add(new ChatRoom
                {
                    roomId = room.Id,
                    roomName = room.Name,
                    roomDescription = room.Description
                });
            }

            return result;
        }

        public async Task<ChatRoom> CreateNewRoom(string name, string description)
        {
            ChatRoomDTO chatRoomDTO = new ChatRoomDTO()
            {
                Name = name,
                Description = description
            };

            var data = new StringContent(JsonConvert.SerializeObject(chatRoomDTO), System.Text.Encoding.UTF8, "application/json");

            HttpClient httpClient = _httpClientFactory.CreateClient("ChatAppApi");
            var roomResponse = await httpClient.PostAsync($"{_configuration.GetSection("API").Value}/api/chat/rooms", data);

            if (roomResponse.IsSuccessStatusCode)
            {
                ChatRoomDTO newChatRoomDTO = JsonConvert.DeserializeObject<ChatRoomDTO>(await roomResponse.Content.ReadAsStringAsync());

                ChatRoom room = new ChatRoom()
                {
                    roomId = newChatRoomDTO.Id,
                    roomName = newChatRoomDTO.Name,
                    roomDescription = newChatRoomDTO.Description
                };

                return room;
            }
            else
            {
                return null;
            }
        }

        public async Task<ChatRoom> JoinChatRoom(int chatRoomId)
        {


            var data = new StringContent(JsonConvert.SerializeObject(chatRoomId), System.Text.Encoding.UTF8, "application/json");

            HttpClient httpClient = _httpClientFactory.CreateClient("ChatAppApi");
            var roomResponse = await httpClient.PostAsync($"{_configuration.GetSection("API").Value}/api/chat/rooms/join", data);

            if (roomResponse.IsSuccessStatusCode)
            {
                ChatRoomDTO newChatRoomDTO = JsonConvert.DeserializeObject<ChatRoomDTO>(await roomResponse.Content.ReadAsStringAsync());

                ChatRoom room = new ChatRoom()
                {
                    roomId = newChatRoomDTO.Id,
                    roomName = newChatRoomDTO.Name,
                    roomDescription = newChatRoomDTO.Description
                };

                return room;
            }
            else
            {
                return null;
            }
        }
    }
}
