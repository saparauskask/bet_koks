using Microsoft.AspNetCore.Mvc;

namespace OnlineNotes.Services.OpenAIServices
{
    public interface IOpenAIService
    {
        Task<string> CompleteSentence(string input);
    }
}
