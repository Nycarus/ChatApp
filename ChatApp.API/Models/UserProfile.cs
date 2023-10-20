using ChatApp.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public int UserId { get; set; }

        public virtual ICollection<ChatRoomUsers> ChatRoomUsers { get; set; } = new List<ChatRoomUsers>();
    }
}
