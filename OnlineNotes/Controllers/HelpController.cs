﻿using Microsoft.AspNetCore.Authorization;
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
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetChatHistory()
        {
            var messages = _chatBotService.GetChatHistory();
            return Json(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage))
            {
                return BadRequest("Message is empty.");
            }

            var response = await _chatBotService.GenerateResponse(userMessage);

            return Content(response.ToString(), "text/plain");
        }
    }
}
