namespace ChatApp.Client.Data
{
    public class ChatRoom
    {
        public int roomId;
        public string roomName = null!;
        public string roomDescription = null!;
        public List<string> messages = new List<string>();
    }
}
