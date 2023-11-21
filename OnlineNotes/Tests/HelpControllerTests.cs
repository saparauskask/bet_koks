/*
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.ContentModel;
using OnlineNotes.Controllers;
using OnlineNotes.Services.OpenAIServices;
using Xunit;

namespace OnlineNotes.Tests
{
    public class HelpControllerTests
    {
        [Fact]
        public async Task SendMessageAsync_ValidMessage_ReturnsContentResult()
        {
            // Arrange
            var chatBotServiceMock = new Mock<ChatBotService>();
            chatBotServiceMock.Setup(service => service.GenerateResponse(It.IsAny<string>()))
                .ReturnsAsync("MockedResponse");

            var controller = new HelpController(chatBotServiceMock.Object);

            // Act
            var result = await controller.SendMessageAsync("TestMessage") as ContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MockedResponse", result.Content);
            Assert.Equal("text/plain", result.ContentType);
        }
    }
}
*/