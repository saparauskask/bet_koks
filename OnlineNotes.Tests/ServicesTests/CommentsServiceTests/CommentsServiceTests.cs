using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Requests.Comments;
using OnlineNotes.Services.CommentsServices;
using OnlineNotes.Services.RatingServices;
using OpenAI_API.Moderation;

namespace OnlineNotes.Tests.ServicesTests.CommentsServiceTests
{
    public class CommentsServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<CommentsService>> _mockLogger;
        private readonly Mock<ReferencesRepository> _mockRefRep;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly CommentsService _service;

        public CommentsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<CommentsService>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);

            _mockRefRep = new Mock<ReferencesRepository>(_context, A.Fake<IHttpContextAccessor>());
            _service = new CommentsService(_mockRefRep.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CommentsService_CreateCommentAsync_ReturnsTrue()
        {
            // Arrange
            var commentRequest = new CreateCommentRequest
            {
                Contents = "Test Comment",
                NoteId = 1
            };

            // Act
            var result = await _service.CreateCommentAsync(commentRequest);

            // Assert
            Assert.True(result);
            Assert.Equal(1, _context.Comment.Count());
        }

        [Fact]
        public async Task CommentsService_CreateCommentAsync_InvalidRefRep_ReturnsFalse()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, null);
            var invalidService = new CommentsService(mockInvalidRefRep.Object, _mockLogger.Object);

            var commentRequest = new CreateCommentRequest
            {
                Contents = "Test Comment",
                NoteId = 1
            };

            // Act
            var result = await invalidService.CreateCommentAsync(commentRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CommentsService_DeleteCommentAsync_ReturnsTrue()
        {
            // Arrange
            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            _context.Comment.Add(comment);
            _context.SaveChanges();

            var deleteCommentRequest = new DeleteCommentRequest
            {
                Id = comment.Id
            };

            // Act
            var result = await _service.DeleteCommentAsync(deleteCommentRequest);

            // Assert
            Assert.True(result);
            Assert.Equal(0, await _context.Comment.CountAsync());
        }

        [Fact]
        public async Task CommentsService_DeleteCommentAsync_CommentDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            _context.Comment.Add(comment);
            _context.SaveChanges();

            var deleteCommentRequest = new DeleteCommentRequest
            {
                Id = 2
            };

            // Act
            var result = await _service.DeleteCommentAsync(deleteCommentRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CommentsService_GetCommentByIdAsync_WithValidId_ReturnsComment()
        {
            // Arrange
            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            _context.Comment.Add(comment);
            _context.SaveChanges();

            // Act
            var result = await _service.GetCommentByIdAsync(comment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Comment", result!.Contents);
        }

        [Fact]
        public async Task CommentsService_GetCommentByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var resultEmpty = await _service.GetCommentByIdAsync(1);
            var resultInvalid = await _service.GetCommentByIdAsync(-1);
            var resultNull = await _service.GetCommentByIdAsync(null);

            // Assert
            Assert.Null(resultEmpty);
            Assert.Null(resultInvalid);
            Assert.Null(resultNull);
        }

        [Fact]
        public async Task CommentsService_GetNoteIdFromCommentId_WithValidCommentId_ReturnsNoteId()
        {
            // Arrange
            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            _context.Comment.Add(comment);
            _context.SaveChanges();

            // Act
            var result = await _service.GetNoteIdFromCommentId(comment.Id);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CommentsService_GetCommentByIdAsync_WithInvalidRefRep_ReturnsNull()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, null);
            var invalidService = new CommentsService(mockInvalidRefRep.Object, _mockLogger.Object);

            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            _context.Comment.Add(comment);
            _context.SaveChanges();

            // Act
            var result = await invalidService.GetCommentByIdAsync(comment.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ComentsService_GetNoteIdFromCommentId_WithInvalidCommentId_ReturnsZero()
        {
            // Act
            var result = await _service.GetNoteIdFromCommentId(1);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
