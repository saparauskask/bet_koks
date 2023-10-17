namespace OnlineNotes.Services.OpenAIServices
{
    public interface IOpenAIService
    {
        Task<string> CompleteSentence(string input);
        Task<string> CompleteHelpRequest(string input = "Can you help me?");
    }
}
