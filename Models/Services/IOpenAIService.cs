namespace OnlineNotes.Models.Services;

public interface IOpenAIService
{
    Task<string> CompleteSentence(string sentence);
}
