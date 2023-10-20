using ChatApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.API.Models
{
    public class ChatRoomUsers
    {
        [Key]
        public int Id { get; set; }
        public ChatRoom ChatRoom { get; set; } = null!;
        public int ChatRoomId { get; set; }
        public UserProfile UserProfile { get; set; } = null!;
        public int UserProfileId { get; set;}
    }
}
