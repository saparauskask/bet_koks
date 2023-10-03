using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models;
using OpenAI_API;

namespace OnlineNotes.Controllers
{

    public class HelpController : Controller
    {
        private readonly string OpenAIapiKey = "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";
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> HelpButtonAction(string input)
        {
            string completionResult = await CompleteSentence(OpenAIapiKey, input);

            return Content(completionResult, "text/plain");
        }

        public async Task<string> CompleteSentence(string OpenAIapiKey, string input)
        {
            var api = new OpenAI_API.OpenAIAPI(OpenAIapiKey);
            var result = await api.Completions.GetCompletion(input);
            return result;
        }
    }
}
