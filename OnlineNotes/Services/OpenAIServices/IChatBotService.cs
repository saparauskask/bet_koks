using OnlineNotes.Data.ChatHistorySaver;
using OnlineNotes.Models;
using OpenAI_API.Chat;
using OpenAI_API;

namespace OnlineNotes.Services.OpenAIServices
{
    public interface IChatBotService
    {
        public void AddUserMessage(string text);
        public void AddAIMessage(string text);
        void LoadChatHistory(List<ChatGptMessage> Messages);
        List<ChatGptMessage> GetChatHistory();
        void ClearChatHistory();
        Task<string> GenerateResponse(string text);

    }
}
