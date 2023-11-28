using FakeItEasy;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Controllers;
using OnlineNotes.Data.ChatHistorySaver;
using OnlineNotes.Data;
using OnlineNotes.Services.OpenAIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models;

namespace OnlineNotes.Tests.ControllersTests
{
    public class HelpControllerTests
    {
        private readonly IChatBotService _chatBotService;
        private readonly HelpController _helpController;

        public HelpControllerTests()
        {
            //Dependencies
            _chatBotService = A.Fake<IChatBotService>();

            //SUT
            _helpController = new HelpController(_chatBotService);
        }

        [Theory]
        [InlineData("Hello!")]
        public void HelpController_SendMessage_ReturnsSuccess(string text)
        {
            // Arrange
            var response = "fake response";
            string input = "fake input";
            A.CallTo(() => _chatBotService.GenerateResponse(input)).Returns(response);
            // Act
            var result = _helpController.SendMessage(text);
            // Assert
            Assert.IsAssignableFrom<Task<IActionResult>>(result);
        }

        [Theory]
        [InlineData("")]
        public void HelpController_SendMessage_BadRequest(string text)
        {
            // Arrange
            // Act
            var result = _helpController.SendMessage(text);
            // Assert
            Assert.IsType<Task<IActionResult>>(result);
        }

        [Fact]
        public void HelpController_GetChatHistory_ReturnsSuccess()
        {
            // Arrance
            var messages = A.Fake<List<ChatGptMessage>>();
            A.CallTo(() => _chatBotService.GetChatHistory()).Returns(messages);
            // Act
            var result = _helpController.GetChatHistory();
            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void HelpController_Index_Success()
        {
            // Arrange
            // Act
            var result = _helpController.Index();
            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void HelpController_ClearChatHistory_Success()
        {
            //Arrange
            A.CallTo(() => _chatBotService.ClearChatHistory());
            //Act
            var result = _helpController.ClearChatHistory();
            //Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }
    }
}
