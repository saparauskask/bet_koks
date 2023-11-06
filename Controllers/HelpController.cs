using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Services.OpenAIServices;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class HelpController : Controller
    {
        private readonly IOpenAIService _openAIService;

        public HelpController(IOpenAIService openAIService)
        {
            //implement sessions
            _openAIService = openAIService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> HelpButtonAction(string input)
        {
            string completionResult;
            if (!string.IsNullOrEmpty(input))
            {
                ChatGPTConversation conversation = new ChatGPTConversation(DateTime.Now);
                completionResult = await conversation.GenerateResponse(input);
            }
            else
            {
                completionResult = await _openAIService.CompleteHelpRequest();
            }
            return Content(completionResult, "text/plain");
        }
    }
}
