using OnlineNotes.Data;
using OpenAI_API;

namespace OnlineNotes.Services.OpenAIServices
{
    public class OpenAIService : IOpenAIService
    {
        readonly OpenAIAPI Api;

        public OpenAIService()
        {
            var apiKey = FileRepository.ReadApiKey();
            Api = new OpenAIAPI(apiKey?.Key);
        }

        public async Task<string> CompleteHelpRequest(string input = "Can you help me?")
        {
            try
            {
                var result = await Api.Completions.CreateCompletionAsync(input, temperature: 1.0);
                return result.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Something went wrong, the request could not be completed";
            }
        }

        public async Task<string> CompleteSentence(string input = "Can you help me?")
        {
            try
            {
                var result = await Api.Completions.CreateCompletionAsync(input, temperature: 0.5);
                return result.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Something went wrong, the request could not be completed";
            }
        }
    }
}
