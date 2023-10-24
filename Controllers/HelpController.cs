using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Services.OpenAIServices;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class HelpController : Controller
    {
        private readonly IOpenAIService _openAIService;
        private List<object> _conversationList;

        public HelpController(IOpenAIService openAIService)
        {
            //implement sessions
            _openAIService = openAIService;
            _conversationList = new List<object>();
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

                _conversationList.Add(conversation);
            }
            else
            {
                completionResult = await _openAIService.CompleteHelpRequest();
            }
            return Content(completionResult, "text/plain");
        }
        public ChatGPTConversation RetrieveConversation(int index)
        {
                if (index >= 0 && index < _conversationList.Count)
                {
                    ChatGPTConversation conversation = (ChatGPTConversation)_conversationList[index];
                    return conversation;
                }
                else
                {
                    return new ChatGPTConversation(default(DateTime));
                }
        }
    }
}
