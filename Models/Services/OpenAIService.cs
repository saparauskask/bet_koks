using Microsoft.Extensions.Options;

namespace OnlineNotes.Models.Services;

public class OpenAIService : IOpenAIService
{
    private readonly OpenAIConfig _openAIConfig;

    public OpenAIService(IOptionsMonitor<OpenAIConfig> optionsMonitor)
    {
        _openAIConfig = optionsMonitor.CurrentValue;
    }
    public async Task<string> CompleteSentence(string sentence)
    {
        //api instance
        var api = new OpenAI_API.OpenAIAPI(_openAIConfig.Key);
        var result = await api.Completions.GetCompletion(sentence);
        return result;
    }
}

