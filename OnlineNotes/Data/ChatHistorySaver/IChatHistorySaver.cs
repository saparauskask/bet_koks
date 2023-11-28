using OnlineNotes.Models;

namespace OnlineNotes.Data.ChatHistorySaver
{
    public interface IChatHistorySaver
    {
        public void AddMessage(ChatGptMessage message);
        public List<ChatGptMessage> GetPendingMessages();
        public List<ChatGptMessage> getAllChatMessagesFromDb();
        public void ClearChatHistory();
    }
}
