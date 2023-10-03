using OpenAI_API;

namespace OnlineNotes.Models
{
    public class OpenAIService
    {
        OpenAI_API.OpenAIAPI api;
        public OpenAIService() 
        {
            api = new OpenAI_API.OpenAIAPI("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");
        }
        public async Task<string> CompleteSentence()
        {
            var result = await api.Completions.GetCompletion("One Two Three One Two");
            return result;
        }
    }
}
