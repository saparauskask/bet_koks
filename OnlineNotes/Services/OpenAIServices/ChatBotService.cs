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
        private readonly ApplicationDbContext _context;

        public ChatBotService(ApplicationDbContext context)
        {
            _context = context;
            var apiKey = FileRepository.ReadApiKey();
            _api = new OpenAIAPI(apiKey?.Key);

            chat = _api.Chat.CreateConversation();
            LoadChatHistory(_context.ChatMessages.ToList());
        }

        public async Task AddUserMessageAsync(string text)
        {
            var userChatMessage = new ChatGptMessage { Content = text, IsUser = true, Timestamp = DateTime.Now };
            _context.ChatMessages.Add(userChatMessage);
            await _context.SaveChangesAsync();

            chat.AppendUserInput(text);
        }

        
        public async Task AddAIMessage(string text)
        {
            var botChatMessage = new ChatGptMessage { Content = text, IsUser = false, Timestamp = DateTime.Now };
            _context.ChatMessages.Add(botChatMessage);
            await _context.SaveChangesAsync();

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
            return _context.ChatMessages.ToList();
        }

        public async Task<string> GenerateResponse(string text)
        {
            await AddUserMessageAsync(text);

            var response = await chat.GetResponseFromChatbotAsync();

            await AddAIMessage(response.ToString());
            return response.ToString();
        }
    }
}
