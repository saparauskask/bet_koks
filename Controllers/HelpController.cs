using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Services.OpenAIServices;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class HelpController : Controller
    {
        private readonly IOpenAIService _openAIService;
        private ChatGPTConversation _conversation;

        public HelpController(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
            _conversation = new ChatGPTConversation(DateTime.Now);
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
                completionResult = await _conversation.GenerateResponse(input);
            }
            else
            {
                completionResult = await _openAIService.CompleteHelpRequest();
            }
            return Content(completionResult, "text/plain");
        }
    }
}
