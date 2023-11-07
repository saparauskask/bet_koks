using OnlineNotes.Services.OpenAIServices;

namespace OnlineNotes.Data
{
    public static class ChatHistory
    {
        private static readonly List<ChatGPTMessage> Messages = new List<ChatGPTMessage>();

        public static void AddMessage(ChatGPTMessage message)
        {
            Messages.Add(message);
        }

        public static List<ChatGPTMessage> GetMessages()
        {
            return Messages;
        }
    }
}
