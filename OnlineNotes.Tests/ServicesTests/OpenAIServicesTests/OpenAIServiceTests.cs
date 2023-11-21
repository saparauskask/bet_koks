using OnlineNotes.Services.OpenAIServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnlineNotes.Tests.ServicesTests.OpenAIServicesTests
{
    public class OpenAIServiceTests
    {
        [Theory]
        [InlineData("Hello")]
        public void OpenAiService_CompleteHelpRequest_returnString(string input)
        {
            // Arrange
            var service = new OpenAIService();
            bool containsLetters = Regex.IsMatch(input, "[a-zA-Z]");
            // Act
            var result = service.CompleteHelpRequest(input);

            // Assert
            Assert.NotNull(result);
            Assert.True(containsLetters, result.ToString());
        }

        [Theory]
        [InlineData("test")]
        public void OpenAiService_CompleteSentence_returnString(string input)
        {
            // Arrange
            var service = new OpenAIService();
            bool containsLetters = Regex.IsMatch(input, "[a-zA-Z]");
            // Act
            var result = service.CompleteSentence(input);

            // Assert
            Assert.NotNull(result);
            Assert.True(containsLetters, result.ToString());
        }
    }
}
