using Microsoft.AspNetCore.Mvc;
using OpenAI_API;

namespace OnlineNotes.Services.OpenAIServices
{
    public class OpenAIService : IOpenAIService
    {
        readonly OpenAIAPI api;

        public OpenAIService()
        {
            api = new OpenAIAPI(ReadApiKey());
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

       private string? ReadApiKey()
       {
            string apiKeyFilePath = ".env";
            try
            {
                using (FileStream fileStream = new FileStream(apiKeyFilePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string apiKey = reader.ReadToEnd().Trim();

                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        return apiKey;
                    }
                    else
                    {
                        Console.WriteLine("API Key is empty or not found in the file.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }  
    }
}
