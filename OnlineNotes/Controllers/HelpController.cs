using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Services.OpenAIServices;
using Newtonsoft.Json;
using OnlineNotes.Data;
using OnlineNotes.Models;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class HelpController : Controller
    {
        private IChatBotService _chatBotService;
        public HelpController(IChatBotService chatBotService)
        {
            _chatBotService = chatBotService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetChatHistory()
        {
            List<ChatGptMessage> messages = _chatBotService.GetChatHistory();
            return Json(messages);
        }

        [HttpPost]
        public IActionResult ClearChatHistory()
        {
            _chatBotService.ClearChatHistory();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return BadRequest("Message is empty.");
            }

            string response = await _chatBotService.GenerateResponse(userMessage);

            return Content(response.ToString(), "text/plain");
        }
    }
}
