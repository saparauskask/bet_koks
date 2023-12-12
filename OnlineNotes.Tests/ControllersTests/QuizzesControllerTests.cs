using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Controllers;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Quizzes;
using OnlineNotes.Models.Requests.Quiz;
using OnlineNotes.Services.NotesServices;
using OnlineNotes.Services.QuizzesServices;
using System.Security.Claims;

namespace OnlineNotes.Tests.ControllersTests
{
    public class QuizzesControllerTests
    {
        [Fact]
        public async Task Index_ReturnsViewWithQuizzes()
        {
            // Arrange
            var userManager = MockUserManager();
            var logger = Mock.Of<ILogger<QuizzesController>>();
            var quizzesService = Mock.Of<IQuizzesService>(
                s => s.GetAllQuizzesToListAsync() == Task.FromResult<IEnumerable<Quiz>>(new List<Quiz> { new Quiz() })
            );
            var notesService = Mock.Of<INotesService>();

            var controller = new QuizzesController(userManager, logger, quizzesService, notesService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
            };

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Quiz>>(viewResult.ViewData.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Index_ReturnsNotFoundWhenQuizzesAreNull()
        {
            // Arrange
            var userManager = MockUserManager();
            var logger = Mock.Of<ILogger<QuizzesController>>();
            var quizzesService = Mock.Of<IQuizzesService>(
                s => s.GetAllQuizzesToListAsync() == Task.FromResult<IEnumerable<Quiz>>(null)
            );
            var notesService = Mock.Of<INotesService>();

            var controller = new QuizzesController(userManager, logger, quizzesService, notesService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
            };

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_POST_RedirectsToQuizAttemptOnSuccessfulCreation()
        {
            // Arrange
            var userManager = MockUserManager();
            var logger = Mock.Of<ILogger<QuizzesController>>();
            var quizzesService = Mock.Of<IQuizzesService>(s => s.CreateQuizAsync(It.IsAny<CreateQuizRequest>()) == Task.FromResult<int?>(1));
            var notesService = Mock.Of<INotesService>();

            var controller = new QuizzesController(userManager, logger, quizzesService, notesService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
            };

            var quiz = new Quiz
            {
                UserId = "userId",
                CreationDate = DateTime.Now,
                Title = "Test Quiz",
                NoteContents = "Test Quiz Contents",
                Difficulty = QuizDifficulty.Easy,
                QuestionsCount = 3
            };

            // Act
            var result = await controller.Create(quiz);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("QuizAttempt", redirectToActionResult.ActionName);
            Assert.Equal(1, redirectToActionResult.RouteValues["id"]);
        }

        [Fact]
        public async Task QuizAttempt_ReturnsViewWithQuiz()
        {
            // Arrange
            var userManager = MockUserManager();
            var logger = Mock.Of<ILogger<QuizzesController>>();
            var quizzesService = Mock.Of<IQuizzesService>(s => s.GetQuizByIdAsync(It.IsAny<int>()) == Task.FromResult(new Quiz()));
            var notesService = Mock.Of<INotesService>();

            var controller = new QuizzesController(userManager, logger, quizzesService, notesService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
            };

            // Act
            var result = await controller.QuizAttempt(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Quiz>(viewResult.Model);
        }

        [Fact]
        public async Task QuizAttempt_ReturnsNotFoundWhenQuizIsNull()
        {
            // Arrange
            var userManager = MockUserManager();
            var logger = Mock.Of<ILogger<QuizzesController>>();
            var quizzesService = Mock.Of<IQuizzesService>(s => s.GetQuizByIdAsync(It.IsAny<int>()) == Task.FromResult<Quiz>(null));
            var notesService = Mock.Of<INotesService>();

            var controller = new QuizzesController(userManager, logger, quizzesService, notesService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
            };

            // Act
            var result = await controller.QuizAttempt(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsRedirectToActionOnSuccessfulDeletion()
        {
            // Arrange
            var userManager = MockUserManager();
            var logger = Mock.Of<ILogger<QuizzesController>>();
            var quizzesService = Mock.Of<IQuizzesService>(s => s.DeleteQuizAsync(It.IsAny<int?>()) == Task.FromResult(true));
            var notesService = Mock.Of<INotesService>();

            var controller = new QuizzesController(userManager, logger, quizzesService, notesService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
            };

            // Act
            var result = await controller.Delete(1);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundWhenDeletionFails()
        {
            // Arrange
            var userManager = MockUserManager();
            var logger = Mock.Of<ILogger<QuizzesController>>();
            var quizzesService = Mock.Of<IQuizzesService>(s => s.DeleteQuizAsync(It.IsAny<int?>()) == Task.FromResult(false));
            var notesService = Mock.Of<INotesService>();

            var controller = new QuizzesController(userManager, logger, quizzesService, notesService)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() } }
            };

            // Act
            var result = await controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        private UserManager<IdentityUser> MockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new UserManager<IdentityUser>(store.Object, null, null, null, null, null, null, null, null);
        }
    }
}
