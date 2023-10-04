using Microsoft.AspNetCore.Mvc;
using OpenAI_API;

namespace OnlineNotes.Services.OpenAIServices
{
    public class OpenAIService : IOpenAIService
    {
        OpenAIAPI api;

        public OpenAIService()
        {
            // TODO: Hide the API key
            api = new OpenAIAPI("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");
        }

        public async Task<string> CompleteSentence(string input)
        {
            string? result = await api.Completions.GetCompletion(input);
            return result;
        }
    }
}
