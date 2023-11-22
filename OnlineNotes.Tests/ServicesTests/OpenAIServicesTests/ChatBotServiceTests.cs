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
using Microsoft.EntityFrameworkCore.Update;

namespace OnlineNotes.Tests.ServicesTests.OpenAIServicesTests
{
    public class ChatBotServiceTests
    {
        private readonly ChatBotService _chatBotService;
        private readonly ChatHistorySaver _chatHistorySaver;

        public ChatBotServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);
            try 
            {
                ChatHistorySaver.Initialize(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            _chatHistorySaver = ChatHistorySaver.Instance;
            _chatBotService = new ChatBotService();
        }

        [Theory]
        [InlineData("human test")]
        public void ChatBotService_AddUserMessage_MessageCountplusOne(string text)
        {
            // Arrange
            int messagesInDbCountBefore = _chatBotService.GetChatHistory().Count;
            // Act
            _chatBotService.AddUserMessage(text);
            int messageInDbCountAfter = _chatHistorySaver.GetPendingMessages().Count;
            _chatHistorySaver.ClearChatHistory();
            // Assert
            Assert.True(messageInDbCountAfter > messagesInDbCountBefore);
        }
        
        [Theory]
        [InlineData("robot test")]
        public void ChatBotService_AddAIMessage_MessageCountplusOne(string text)
        {
            // Arrange
            int messagesInDbCountBefore = _chatBotService.GetChatHistory().Count;
            // Act
            _chatBotService.AddAIMessage(text);
            int messageInDbCountAfter = _chatHistorySaver.GetPendingMessages().Count;
            _chatHistorySaver.ClearChatHistory();
            // Assert
            Assert.True(messageInDbCountAfter > messagesInDbCountBefore);
        }

        [Theory]
        [InlineData("hello!")]
        public void ChatBotService_GenerateResponse_returnsString(string text)
        {
            // Arrange
            int messagesInDbCountBefore = _chatBotService.GetChatHistory().Count;
            // Act
            var result = _chatBotService.GenerateResponse(text);
            int messageInDbCountAfter = _chatHistorySaver.GetPendingMessages().Count;
            _chatHistorySaver.ClearChatHistory();
            // Assert
            Assert.NotNull(result);
            Assert.True(messagesInDbCountBefore < messageInDbCountAfter);
        }

        [Fact]
        public void ChatBotService_ClearChatHistory_IsZero()
        {
            // Arrange
            _chatBotService.AddUserMessage("1");
            _chatBotService.AddAIMessage("2");
            int messagesInDbCountBefore = _chatHistorySaver.GetPendingMessages().Count;
            // Act
            _chatBotService.ClearChatHistory();
            int messageInDbCountAfter = _chatBotService.GetChatHistory().Count;
            // Assert
            Assert.True(messageInDbCountAfter == 0);
            Assert.True(messagesInDbCountBefore > messageInDbCountAfter);
        }

        [Fact]
        public void ChatBotService_LoadChatHistory_NotNull()
        {
            // Arrange
            var messageList = new List<ChatGptMessage>();
            messageList.Add(new ChatGptMessage { Content = "x is equal to 2", IsUser = false });
            messageList.Add(new ChatGptMessage { Content = "x is equal to 2", IsUser = true });
            // Act
            _chatBotService.LoadChatHistory(messageList);
            var result = _chatBotService.GenerateResponse("What is x + x?");
            // Assert
            Assert.NotNull(result);
            //Assert.Contains("4", result.ToString());
        }


    }


}
