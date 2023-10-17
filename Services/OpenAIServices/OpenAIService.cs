using OnlineNotes.Data;
using OpenAI_API;

namespace OnlineNotes.Services.OpenAIServices
{
    public class OpenAIService : IOpenAIService
    {
        readonly OpenAIAPI api;

        public OpenAIService()
        {
            var apiKey = FileRepository.ReadApiKey();
            api = new OpenAIAPI(apiKey?.Key);
        }

        public async Task<string> CompleteSentence(string input)
        {
            try
            {
                string? result = await api.Completions.GetCompletion(input);
                return result;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return "Something went wrong, the request could not be completed";
            }
        }
    }
}
