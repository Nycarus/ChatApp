namespace ChatApp.Models
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        public UserModel UserModel { get; set; } = null!;
    }
}
