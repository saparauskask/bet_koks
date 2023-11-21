using Microsoft.AspNetCore.SignalR;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OpenAI_API;
using OpenAI_API.Chat;

namespace OnlineNotes.Services.OpenAIServices
{
    public class ChatBotService : IChatBotService
    {
        private OpenAIAPI _api;
        private Conversation chat;
        ChatHistorySaver _chatHistorySaver;

        public ChatBotService()
        {
            var apiKey = FileRepository.ReadApiKey();
            _api = new OpenAIAPI(apiKey?.Key);

            _chatHistorySaver = ChatHistorySaver.Instance;

            chat = _api.Chat.CreateConversation();
            LoadChatHistory(_chatHistorySaver.getAllChatMessagesFromDb());
        }

        public void AddUserMessageAsync(string text)
        {
            var userChatMessage = new ChatGptMessage { Content = text, IsUser = true, Timestamp = DateTime.Now };

            _chatHistorySaver.AddMessage(userChatMessage);
            chat.AppendUserInput(text);
        }

        
        public void AddAIMessage(string text)
        {
            var botChatMessage = new ChatGptMessage { Content = text, IsUser = false, Timestamp = DateTime.Now };

            _chatHistorySaver.AddMessage(botChatMessage);
            chat.AppendSystemMessage(text);
        }
        
        
        public void LoadChatHistory(List<ChatGptMessage> Messages)
        {
            if (Messages != null)
            {
                foreach (ChatGptMessage message in Messages)
                {
                    if (message.IsUser) { chat.AppendUserInput(message.Content); }
                    if (!message.IsUser) { chat.AppendExampleChatbotOutput(message.Content); }
                }
            }
        }

        public List<ChatGptMessage> GetChatHistory()
        {
            return _chatHistorySaver.getAllChatMessagesFromDb();
        }

        public void ClearChatHistory()
        {
            _chatHistorySaver.ClearChatHistory();
        }

        public async Task<string> GenerateResponse(string text)
        {
            AddUserMessageAsync(text);

            var response = await chat.GetResponseFromChatbotAsync();

            AddAIMessage(response.ToString());
            return response.ToString();
        }
    }
}
