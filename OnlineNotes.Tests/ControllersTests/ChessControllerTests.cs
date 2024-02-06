using FakeItEasy;
using OnlineNotes.Controllers;

namespace OnlineNotes.Tests.ControllersTests
{
    public class ChessControllerTests
    {
        private readonly ChessController _chessController;
        private readonly HttpClient _httpClient;
        public ChessControllerTests()
        {
            _httpClient = A.Fake<HttpClient>();

            // Configure the behavior of fakeHttpClient if needed

            _chessController = new ChessController(null, _httpClient);
        }

        [Fact]
        public async Task ChessController_Index_ReturnsSuccessAsync()
        {
            // Arrange
            var expectedHtmlBoard = "Mocked HTML Board";

            //A.CallTo(() => _httpClient.GetAsync(A<Uri>._))
                //.Returns(new HttpResponseMessage
                //{
                    //StatusCode = HttpStatusCode.OK,
                    //Content = new StringContent(expectedHtmlBoard),
                //});

            // Act
            //var result = await _chessController.Index();

            // Assert
            //var viewResult = Assert.IsType<ViewResult>(result);
        }
    }
}
