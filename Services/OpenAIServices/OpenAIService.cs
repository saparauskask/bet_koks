using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using System.Globalization;

namespace OnlineNotes.Services.OpenAIServices
{
    public class OpenAIService : IOpenAIService
    {
        readonly OpenAIAPI api;
        private record OpenAIAPIKey(string Key, DateTime CreationDate);

        public OpenAIService()
        {
            var apiKey = ReadApiKey();
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

       private OpenAIAPIKey? ReadApiKey()
       {
            string apiKeyFilePath = ".env";
            try
            {
                using (FileStream fileStream = new FileStream(apiKeyFilePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string apiKey = reader.ReadLine()?.Trim() ?? string.Empty; //provides empty string as default value
                    string dateStr = reader.ReadLine()?.Trim() ?? string.Empty;

                    if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(dateStr))
                    {
                        if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime creationDate))
                        {
                            return new OpenAIAPIKey(apiKey, creationDate);
                        }
                        else
                        {
                            Console.WriteLine("Failed to parse the date in the file.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("API Key or Date is empty or not found in the file.");
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
