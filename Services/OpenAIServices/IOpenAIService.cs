namespace OnlineNotes.Services.OpenAIServices
{
    public interface IOpenAIService
    {
        Task<string> CompleteSentence(string input);
    }
}
