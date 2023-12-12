using OnlineNotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineNotes.Tests.ModelsTests
{
    public class ChatGptMessageTests
    {
        public ChatGptMessageTests() 
        {

        }

        [Fact]
        public void ChatGptMeessage_Success()
        {
            //Arange
            var message = new ChatGptMessage();
            //Act
            message.Id = 1;
            message.Content = "test";
            message.IsUser = true;
            message.Timestamp = DateTime.Now;
            //Assert
            Assert.NotNull(message);
            Assert.NotNull(message.Content);
        }
    }
}
