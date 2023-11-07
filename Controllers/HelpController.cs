using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Services.OpenAIServices;
using Newtonsoft.Json;
using OnlineNotes.Data;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class HelpController : Controller
    {
        private ChatBotService _chatBotService;
        public HelpController(ChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
            _chatBotService.LoadChatHistory(ChatHistory.GetMessages());
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return BadRequest("Message is empty.");
            }

            ChatHistory.AddMessage(new ChatGPTMessage(userMessage, isUser: true));

            var response = await _chatBotService.GenerateResponse(userMessage);
            ChatHistory.AddMessage(new ChatGPTMessage(response.ToString(), isUser: false));

            return Content(response.ToString(), "text/plain");
        }
    }
}
