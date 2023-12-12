using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Quizzes;
using OnlineNotes.Models.Requests.Quiz;
using OnlineNotes.Services.OpenAIServices;
using OnlineNotes.Services.QuizzesServices;

namespace OnlineNotes.Tests.ServicesTests.QuizzesServiceTests
{
    public class QuizzesServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<QuizzesService>> _mockLogger;
        private readonly Mock<ReferencesRepository> _mockRefRep;
        private readonly Mock<IQuizGeneratorService> _mockQuizGeneratorService;
        private readonly QuizzesService _service;

        public QuizzesServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<QuizzesService>>();
            _mockRefRep = new Mock<ReferencesRepository>(_context, A.Fake<IHttpContextAccessor>());
            _mockQuizGeneratorService = new Mock<IQuizGeneratorService>();

            _service = new QuizzesService(_mockRefRep.Object, _mockLogger.Object, _mockQuizGeneratorService.Object);
        }

        [Fact]
        public async Task QuizzesService_CreateQuizAsync_WithValidQuizRequest_ReturnsQuizId()
        {
            // Arrange
            var quizRequest = new CreateQuizRequest
            {
                UserId = "1234567",
                CreationDate = DateTime.Now,
                Title = "Test Quiz",
                NoteContents = "Test Quiz Contents",
                Difficulty = QuizDifficulty.Easy,
                QuestionsCount = 3
            };
            string fakeReturn = "Q1. What is it\n";
            fakeReturn += " a. This is the option. b. This is the option. c. This is the option.\n";
            fakeReturn += "Correct Answer for question Q4 is: a.";

            _mockQuizGeneratorService.Setup(x => x.FakeGenerateQuiz(It.IsAny<string>())).Returns(fakeReturn);

            // Act
            var result = await _service.CreateQuizAsync(quizRequest);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasValue && result.Value > 0, "The CreateQuizAsync method did not return a valid QuizId.");
            Assert.Single(_context.Quiz);
            Assert.Single(_context.Question);
        }

        [Fact]
        public async Task QuizzesService_CreateQuizAsync_WithInvalidQuizRequest_ReturnsNull()
        {
            // Arrange
            var quizRequest = new CreateQuizRequest
            {
                // Invalid quiz request
                UserId = "1234567",
                CreationDate = DateTime.Now,
                Title = null,
                NoteContents = null,
                Difficulty = QuizDifficulty.Easy,
                QuestionsCount = 3
            };

            // Act
            var result = await _service.CreateQuizAsync(quizRequest);

            // Assert
            Assert.Null(result);
            Assert.Empty(_context.Quiz);
            Assert.Empty(_context.Question);
        }

        [Fact]
        public async Task QuizzesService_DeleteQuizAsync_WithValidId_ReturnsTrue()
        {
            // Arrange
            var quiz = new Quiz("1234567", DateTime.Now, "Test Quiz", "Test Quiz Contents", QuizDifficulty.Easy, 3);
            quiz.Id = 1;

            _context.Quiz.Add(quiz);
            _context.SaveChanges();

            var Id = 1;

            // Act
            var result = await _service.DeleteQuizAsync(Id);

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Quiz);
            Assert.Empty(_context.Question);
        }

        [Fact]
        public async Task QuizzesService_DeleteQuizAsync_WithInvalidId_ReturnsFalse()
        {
            // Arrange
            var Id = 0;

            // Act
            var result = await _service.DeleteQuizAsync(Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task QuizzesService_GetAllQuizzesToListAsync_ReturnsListOfQuizzes()
        {
            // Arrange
            var quizzes = new List<Quiz>
        {
            new Quiz("1234567", DateTime.Now, "Quiz 1", "Contents 1", QuizDifficulty.Easy, 3),
            new Quiz("1234567", DateTime.Now, "Quiz 2", "Contents 2", QuizDifficulty.Moderate, 5)
        };

            _context.Quiz.AddRange(quizzes);
            _context.SaveChanges();

            // Act
            var result = await _service.GetAllQuizzesToListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task QuizzesService_GetQuizByIdAsync_WithValidId_ReturnsQuiz()
        {
            // Arrange
            var quiz = new Quiz("1234567", DateTime.Now, "Test Quiz", "Test Quiz Contents", QuizDifficulty.Easy, 3);
            quiz.Id = 1;

            _context.Quiz.Add(quiz);
            _context.SaveChanges();

            // Act
            var result = await _service.GetQuizByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(quiz.Id, result.Id);
            Assert.Equal(quiz.Title, result.Title);
        }

        [Fact]
        public async Task QuizzesService_GetQuizByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            // Act
            var result = await _service.GetQuizByIdAsync(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task QuizzesService_CreateQuestionsAsync_WithValidGeneratedQuiz_ReturnsTrue()
        {
            // Arrange
            string generatedQuiz = "Q1. What is it\n";
            generatedQuiz += " a. This is the option. b. This is the option. c. This is the option.\n";
            generatedQuiz += "Correct Answer for question Q4 is: a.";
            var quizId = 1;

            // Act
            var result = await _service.CreateQuestionsAsync(generatedQuiz, quizId);

            // Assert
            Assert.True(result);
            Assert.Single(_context.Question);
        }

        [Fact]
        public async Task QuizzesService_CreateQuestionAsync_WithValidQuestionData_ReturnsTrue()
        {
            // Arrange
            var extractedQuestion = "What is it";
            var parsedAnswers = new List<(string AnswerText, bool IsCorrect)>
        {
            ("This is the option.", false),
            ("This is the option.", false),
            ("This is the option.", true)
        };
            var quizId = 1;

            // Act
            var result = await _service.CreateQuestionAsync(extractedQuestion, parsedAnswers, quizId);

            // Assert
            Assert.True(result);
            Assert.Single(_context.Question);
            //Assert.Single(_context.QuestionOptions);
        }
    }
}
