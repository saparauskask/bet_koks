using FakeItEasy;
using Microsoft.AspNetCore.Http;
using OnlineNotes.Controllers;
using OnlineNotes.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineNotes.Tests.ControllersTests
{
    public class ChessControllerTests
    {
        private readonly ChessController _chessController;
        private readonly ReferencesRepository _referencesRepository;
        private readonly HttpClient _fakeHttpClient;
        public ChessControllerTests()
        {
            // Mock dependencies
            _referencesRepository = A.Fake<ReferencesRepository>();
            var httpContextAccessor = A.Fake<IHttpContextAccessor>();
            var httpContext = A.Fake<HttpContext>();
            var request = A.Fake<HttpRequest>();
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);
            A.CallTo(() => httpContext.Request).Returns(request);
            A.CallTo(() => request.Host).Returns(new HostString("example.com"));

            A.CallTo(() => _referencesRepository.httpContextAccessor).Returns(httpContextAccessor);

            _fakeHttpClient = A.Fake<HttpClient>();
            A.CallTo(() => _fakeHttpClient.BaseAddress).Returns(new Uri("https://example.com/api"));

            // Create an instance of the controller with the mocked dependencies
            _chessController = new ChessController(_referencesRepository, _fakeHttpClient);
        }
    }
}
