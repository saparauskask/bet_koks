using Moq;
using OnlineNotes.Data;
using OnlineNotes.Services.OpenAIServices;
using OpenAI_API.Chat;
using OpenAI_API;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineNotes.Models;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using FakeItEasy;
using OnlineNotes.Data.ChatHistorySaver;

namespace OnlineNotes.Tests.ServicesTests.OpenAIServicesTests
{
    public class ChatBotServiceTests
    {
        private readonly ChatBotService _chatBotService;

        public ChatBotServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);
            ChatHistorySaver.Initialize(context);
            _chatBotService = new ChatBotService();
        }

        [Theory]
        [InlineData("test")]
        public void ChatBotService_AddUserMessageAsync_MessageCountplusOne(string text)
        {
            // Arrange
            var messagesInDbCountBefore = _chatBotService.GetChatHistory().Count;
            // Act
            _chatBotService.AddUserMessage(text);
            var messageInDbCountAfter = _chatBotService.GetChatHistory().Count;
            // Assert
        }

        [Fact]
        public void GenerateResponseAsync_AddsUserMessageAndAIMessage()
        {
            // Arrange

            // Act

            // Assert
        }
    }


}
