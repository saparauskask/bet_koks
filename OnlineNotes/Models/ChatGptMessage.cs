namespace OnlineNotes.Models
{
    public class ChatGptMessage
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
