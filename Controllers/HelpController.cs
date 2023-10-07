using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models;
using OnlineNotes.Services.OpenAIServices;
using OpenAI_API;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class HelpController : Controller
    {
        private readonly IOpenAIService _openAIService;

        public HelpController(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> HelpButtonAction(string input)
        {
            string completionResult = await _openAIService.CompleteSentence(input);
            return Content(completionResult, "text/plain");
        }
    }
}
