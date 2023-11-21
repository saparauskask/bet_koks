namespace OnlineNotes.Services.OpenAIServices
{
    public interface IChatBotService
    {
        Task<string> GenerateResponse(string text);
        public Task AddUserMessageAsync(string text);
        public Task AddAIMessage(string text);
    }
}
