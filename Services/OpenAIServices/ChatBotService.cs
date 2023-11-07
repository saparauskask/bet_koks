using Microsoft.AspNetCore.SignalR;
using OnlineNotes.Data;
using OpenAI_API;
using OpenAI_API.Chat;

namespace OnlineNotes.Services.OpenAIServices
{
    public class ChatBotService : IChatBotService
    {
        private OpenAIAPI _api;
        private Conversation chat;
        //public DateTime StartTime { get; }
        //public DateTime EndTime { get; }
        //public string UserId { get; }
        public List<ChatGPTMessage> Messages { get; }

        public ChatBotService()
        {
            //StartTime = startTime;
            //EndTime = DateTime.MinValue;
            //UserId = userId;

            var apiKey = FileRepository.ReadApiKey();
            _api = new OpenAIAPI(apiKey?.Key);

            Messages = new List<ChatGPTMessage>();
            chat = _api.Chat.CreateConversation();
        }

        public void AddUserMessage(string text)
        {
            /*
            if (EndTime != DateTime.MinValue)
            {
                throw new InvalidOperationException("Conversation is already closed.");
            }
            */

            Messages.Add(new ChatGPTMessage(text, isUser: true));
            chat.AppendUserInput(text);
        }

        public void AddAIMessage(string text)
        {
            /*
            if (EndTime != DateTime.MinValue)
            {
                throw new InvalidOperationException("Conversation is already closed.");
            }
            */

            Messages.Add(new ChatGPTMessage(text, isUser: false));
            chat.AppendSystemMessage(text);
        }

        public void LoadChatHistory(List<ChatGPTMessage> Messages)
        {
            if (Messages != null)
            {
                foreach (ChatGPTMessage message in Messages)
                {
                    if (message.IsUser) { chat.AppendUserInput(message.Text); }
                    if (!message.IsUser) { chat.AppendExampleChatbotOutput(message.Text); }
                }
            }
        }

        public async Task<string> GenerateResponse(string text)
        {
            AddUserMessage(text);

            var response = await chat.GetResponseFromChatbotAsync();

            AddAIMessage(response);
            return response.ToString();
        }
    }
}
