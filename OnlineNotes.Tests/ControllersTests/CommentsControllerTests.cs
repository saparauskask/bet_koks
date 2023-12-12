using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineNotes.Controllers;
using OnlineNotes.Models.Requests.Comments;
using OnlineNotes.Models;
using OnlineNotes.Services.CommentsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineNotes.Tests.ControllersTests
{
    public class CommentsControllerTests
    {
        [Fact]
        public void Create_Get_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var commentsServiceMock = new Mock<ICommentsService>();
            var controller = new CommentsController(commentsServiceMock.Object);
            var noteId = 1;

            // Act
            var result = controller.Create(noteId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(noteId, result.ViewData["Message"]);
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToNoteDetails()
        {
            // Arrange
            var commentsServiceMock = new Mock<ICommentsService>();
            commentsServiceMock.Setup(s => s.CreateCommentAsync(It.IsAny<CreateCommentRequest>())).ReturnsAsync(true);
            var controller = new CommentsController(commentsServiceMock.Object);
            var comment = new CreateCommentRequest { NoteId = 1, Contents = "Test Comment" };

            // Act
            var result = await controller.Create(comment) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Notes", result.ControllerName);
            Assert.Equal(comment.NoteId, result.RouteValues["id"]);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var commentsServiceMock = new Mock<ICommentsService>();
            var controller = new CommentsController(commentsServiceMock.Object);
            var comment = new CreateCommentRequest { NoteId = 1, Contents = null };
            controller.ModelState.AddModelError("Contents", "The Contents field is required.");

            // Act
            var result = await controller.Create(comment) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(comment, result.Model);
        }

        [Fact]
        public async Task Delete_Get_ReturnsViewWithCorrectModel()
        {
            // Arrange
            var commentsServiceMock = new Mock<ICommentsService>();
            commentsServiceMock.Setup(s => s.GetCommentByIdAsync(It.IsAny<int>())).ReturnsAsync(new Comment { Id = 1, Contents = "Test Comment" });
            var controller = new CommentsController(commentsServiceMock.Object);
            var commentId = 1;

            // Act
            var result = await controller.DeleteAsync(commentId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(commentId, result.ViewData["Id"]);
            Assert.Equal("Test Comment", result.ViewData["Contents"]);
        }

        [Fact]
        public async Task DeleteConfirmed_ValidModel_RedirectsToNoteDetails()
        {
            // Arrange
            var commentsServiceMock = new Mock<ICommentsService>();
            commentsServiceMock.Setup(s => s.GetNoteIdFromCommentId(It.IsAny<int>())).ReturnsAsync(1);
            commentsServiceMock.Setup(s => s.DeleteCommentAsync(It.IsAny<DeleteCommentRequest>())).ReturnsAsync(true);
            var controller = new CommentsController(commentsServiceMock.Object);
            var comment = new DeleteCommentRequest { Id = 1 };

            // Act
            var result = await controller.DeleteConfirmed(comment) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("Notes", result.ControllerName);
            Assert.Equal(1, result.RouteValues["id"]);
        }

        [Fact]
        public async Task DeleteConfirmed_InvalidModel_ReturnsNotFound()
        {
            // Arrange
            var commentsServiceMock = new Mock<ICommentsService>();
            var controller = new CommentsController(commentsServiceMock.Object);
            var comment = new DeleteCommentRequest { Id = 0 }; // Invalid model

            // Act
            var result = await controller.DeleteConfirmed(comment) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteAsync_WhenCommentIsNull_ReturnsNotFound()
        {
            // Arrange
            var commentsServiceMock = new Mock<ICommentsService>();
            commentsServiceMock.Setup(s => s.GetCommentByIdAsync(It.IsAny<int>())).ReturnsAsync((Comment)null);
            var controller = new CommentsController(commentsServiceMock.Object);
            var commentId = 1;

            // Act
            var result = await controller.DeleteAsync(commentId) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }
    }

}
