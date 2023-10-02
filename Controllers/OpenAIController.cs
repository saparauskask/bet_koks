using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models.Services;

namespace OnlineNotes.Controllers;

[ApiController]
[Route("[controller")]
public class OpenAIController : ControllerBase
{
    private readonly ILogger<OpenAIController> _logger;
    private readonly IOpenAIService _openAIService;

    public OpenAIController(ILogger<OpenAIController> logger, IOpenAIService openAIService)
    {
        _logger = logger;
        _openAIService = openAIService;
    }

    [HttpPost()]
    [Route("CompleteSentence")]
    public async Task<IActionResult> CompleteSentence(string sentence)
    {
        var result = await _openAIService.CompleteSentence(sentence);
        return Ok(result);
    }
}
