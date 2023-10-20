using System.ComponentModel.DataAnnotations;

namespace ChatApp.API.Models
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<ChatRoomUsers> ChatRoomUsers { get; set; } = new List<ChatRoomUsers>();
    }
}
