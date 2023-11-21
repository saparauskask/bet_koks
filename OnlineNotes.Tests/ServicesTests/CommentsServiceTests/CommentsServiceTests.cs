using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Models.Requests.Comments;
using OnlineNotes.Models;
using OnlineNotes.Services.CommentsServices;

namespace OnlineNotes.Tests.ServicesTests.CommentsServiceTests
{
    public class CommentsServiceTests
    {
        [Fact]
        public async Task CreateCommentAsync_Success()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<CommentsService>>();
            var service = new CommentsService(context, mockLogger.Object);

            var commentRequest = new CreateCommentRequest
            {
                Contents = "Test Comment",
                NoteId = 1
            };

            // Act
            var result = await service.CreateCommentAsync(commentRequest);

            // Assert
            Assert.True(result);
            Assert.Equal(1, context.Comment.Count());
        }

        [Fact]
        public async Task DeleteCommentAsync_Success()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<CommentsService>>();
            var service = new CommentsService(context, mockLogger.Object);

            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            context.Comment.Add(comment);
            context.SaveChanges();

            var deleteCommentRequest = new DeleteCommentRequest
            {
                Id = comment.Id
            };

            // Act
            var result = await service.DeleteCommentAsync(deleteCommentRequest);

            // Assert
            Assert.True(result);
            Assert.Equal(0, context.Comment.Count());
        }

        [Fact]
        public async Task GetCommentByIdAsync_WithValidId_ReturnsComment()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<CommentsService>>();
            var service = new CommentsService(context, mockLogger.Object);

            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            context.Comment.Add(comment);
            context.SaveChanges();

            // Act
            var result = await service.GetCommentByIdAsync(comment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Comment", result.Contents);
        }

        [Fact]
        public async Task GetCommentByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<CommentsService>>();
            var service = new CommentsService(context, mockLogger.Object);

            // Act
            var result = await service.GetCommentByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetNoteIdFromCommentId_WithValidCommentId_ReturnsNoteId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<CommentsService>>();
            var service = new CommentsService(context, mockLogger.Object);

            var comment = new Comment
            {
                Contents = "Test Comment",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            context.Comment.Add(comment);
            context.SaveChanges();

            // Act
            var result = await service.GetNoteIdFromCommentId(comment.Id);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GetNoteIdFromCommentId_WithInvalidCommentId_ReturnsZero()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<CommentsService>>();
            var service = new CommentsService(context, mockLogger.Object);

            // Act
            var result = await service.GetNoteIdFromCommentId(1);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
