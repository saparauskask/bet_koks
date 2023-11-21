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

namespace OnlineNotes.Tests.ServicesTests.OpenAIServicesTests
{
    public class ChatBotServiceTests
    {
        //private readonly ApplicationDbContext _dbContext;

        public ChatBotServiceTests()
        {
            // Set up an in-memory database
            //var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                //.UseInMemoryDatabase(databaseName: "In)
                //.Options;

            //_dbContext = new ApplicationDbContext(options);
        }

            [Fact]
        public void ChatBotService_AddUserMessageAsync_MessageCountplusOne()
        {
            // Arrange
            //int countBefore = ChatHistorySaver.Instance.getAllChatMessagesFromDb().Count;
            // Act
            //_chatBotService.AddUserMessageAsync("test");

            // Assert
            //Assert.Equal(1, countBefore + 1);
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
