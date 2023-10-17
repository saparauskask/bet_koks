using OnlineNotes.Services.OpenAIServices;
using System.Globalization;

namespace OnlineNotes.Data
{
    public struct FileRepository
    {
        public static OpenAIAPIKey? ReadApiKey()
        {
            string apiKeyFilePath = ".env"; // ignored by git
            try
            {
                using (FileStream fileStream = new FileStream(apiKeyFilePath, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string apiKey = reader.ReadLine()?.Trim() ?? string.Empty; // provides an empty string as the default value
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
