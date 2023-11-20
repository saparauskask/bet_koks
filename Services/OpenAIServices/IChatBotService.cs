namespace OnlineNotes.Services.OpenAIServices
{
    public interface IChatBotService
    {
        Task<string> GenerateResponse(string text);
        public void AddUserMessageAsync(string text);
        public void AddAIMessage(string text);
    }
}
