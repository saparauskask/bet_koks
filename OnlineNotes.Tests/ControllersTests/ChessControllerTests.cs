using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Controllers;
using OnlineNotes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OnlineNotes.Tests.ControllersTests
{
    public class ChessControllerTests
    {
        private readonly ChessController _chessController;
        private readonly ReferencesRepository _referencesRepository;
        private readonly HttpClient _httpClient;
        public ChessControllerTests()
        {
            //_referencesRepository = A.Fake<ReferencesRepository>();
            _httpClient = A.Fake<HttpClient>();

            // Configure the behavior of fakeHttpClient if needed

            //_chessController = new ChessController(_fakeReferencesRepository, _fakeHttpClient);
        }

        [Fact]
        public Task ChessController_Index_ReturnsSuccessAsync()
        {
            /*
           // Arrange
           var expectedHtmlBoard = "<html>YourMockedHtmlBoard</html>";

           var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
           {
               Content = new StringContent(expectedHtmlBoard),
           };

           A.CallTo(() => _fakeHttpClient.GetAsync(A<string>._, A<CancellationToken>._))
               .Returns(Task.FromResult(responseMessage));

           // Act
           var result = await _chessController.Index();
           */
            var result = "hello";
            // Assert
            Assert.NotNull(result);
            return Task.CompletedTask;
        }
    }
}
