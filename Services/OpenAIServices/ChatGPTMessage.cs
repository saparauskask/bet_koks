namespace OnlineNotes.Services.OpenAIServices
{
    public struct ChatGPTMessage
    {
        public string Text { get; }
        public bool IsUser { get; }

        public ChatGPTMessage(string text, bool isUser)
        {
            Text = text;
            IsUser = isUser;
        }
    }
}
