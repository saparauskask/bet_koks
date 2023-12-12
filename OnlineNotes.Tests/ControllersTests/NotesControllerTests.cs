using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using OnlineNotes.Controllers;
using OnlineNotes.Exceptions;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;
using OnlineNotes.Services.NotesServices;
using OnlineNotes.Services.OpenAIServices;
using OnlineNotes.Services.RatingServices;
using System.Security.Claims;

namespace OnlineNotes.Tests.ControllersTests
{
    public class NotesControllerTests
    {
        private readonly Mock<IOpenAIService> _openAIServiceMock;
        private readonly Mock<INotesService> _notesServiceMock;
        private readonly Mock<INoteRatingService> _ratingServiceMock;
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<ILogger<NotesService>> _loggerMock;

        public NotesControllerTests()
        {
            _openAIServiceMock = new Mock<IOpenAIService>();
            _notesServiceMock = new Mock<INotesService>();
            _ratingServiceMock = new Mock<INoteRatingService>();

            var userManagerMock = new Mock<UserManager<IdentityUser>>(
                new Mock<IUserStore<IdentityUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<IdentityUser>>().Object,
                new IUserValidator<IdentityUser>[0],
                new IPasswordValidator<IdentityUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<IdentityUser>>>().Object
            );

            _userManagerMock = userManagerMock;
            _userManagerMock
                .Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new IdentityUser { Id = "1234567" });

            _loggerMock = new Mock<ILogger<NotesService>>();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WhenCalled()
        {
            // Arrange
            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public async Task Details_ReturnsViewResult_WhenNoteExists()
        {
            // Arrange
            int noteId = 1;
            var note = new Note("Test title", "Test contents", NoteStatus.Public) 
            { 
                Id = noteId,
                UserId = "1234567"
            };
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ReturnsAsync(note);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Details(noteId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(note, viewResult.Model);
            Assert.Equal(noteId, viewResult.ViewData["NoteId"]);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenNoteDoesNotExist()
        {
            // Arrange
            int noteId = 1;
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ReturnsAsync((Note)null);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Details(noteId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsRedirectToIndex_WhenNoteAccessDeniedExceptionIsThrown()
        {
            // Arrange
            int noteId = 1;
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ThrowsAsync(new NoteAccessDeniedException("1234567", noteId, "Read"));

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Details(noteId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WithUserId_WhenUserExists()
        {
            // Arrange
            var userId = "1234567";
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = userId });

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(userId, viewResult.ViewData["UserId"]);
        }

        [Fact]
        public async Task Create_ReturnsNotFoundResult_WhenUserIsNull()
        {
            // Arrange
            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((IdentityUser)null);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Create();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Filter_SetsFilterStatusAndRedirectsToIndex()
        {
            // Arrange
            var status = NoteStatus.Public;
            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = controller.Filter(status);

            // Assert
            _notesServiceMock.Verify(x => x.SetFilterStatus(status), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Filter_WithoutStatus_RedirectsToIndex()
        {
            // Arrange
            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = controller.Filter(null);

            // Assert
            _notesServiceMock.Verify(x => x.SetFilterStatus(null), Times.Once);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Search_ReturnsViewResult_WithFilteredNotes()
        {
            // Arrange
            var term = "searchTerm";
            var notes = new List<Note> { /* Populate with test notes */ };
            _notesServiceMock.Setup(x => x.GetIndexedNotesToListAsync(term)).ReturnsAsync(notes);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Search(term);

            // Assert
            _notesServiceMock.Verify(x => x.GetIndexedNotesToListAsync(term), Times.Once);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(notes, viewResult.Model);
        }

        [Fact]
        public async Task Search_WithEmptyTerm_RedirectsToIndex()
        {
            // Arrange
            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Search("");

            // Assert
            _notesServiceMock.Verify(x => x.GetIndexedNotesToListAsync(It.IsAny<string>()), Times.Never);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Search_WithWhitespaceTerm_RedirectsToIndex()
        {
            // Arrange
            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Search("   ");

            // Assert
            _notesServiceMock.Verify(x => x.GetIndexedNotesToListAsync(It.IsAny<string>()), Times.Never);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Sort_SetsSortStatusAndRedirectsToIndex()
        {
            // Arrange
            var sortOrder = 1; // Replace with your desired sort order
            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = controller.Sort(sortOrder);

            // Assert
            // Check that SortStatus is set in the _notesService
            _notesServiceMock.Verify(x => x.SetSortStatus(sortOrder), Times.Once);

            // Check that the action redirects to the Index action
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_ReturnsViewResult_WithNoteWhenNoteExists()
        {
            // Arrange
            int noteId = 1;
            var note = new Note("Test title", "Test contents", NoteStatus.Public) 
            { 
                Id = noteId, 
            };
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ReturnsAsync(note);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Edit(noteId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(note, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFoundResult_WhenNoteDoesNotExist()
        {
            // Arrange
            int noteId = 1;
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ReturnsAsync((Note)null);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Edit(noteId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsRedirectToIndex_WhenNoteAccessDeniedExceptionIsThrown()
        {
            // Arrange
            int noteId = 1;
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ThrowsAsync(new NoteAccessDeniedException("1234567", noteId, "Edit"));

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Edit(noteId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ExplainTask_ReturnsContentResultWithCompletionResult()
        {
            // Arrange
            string input = "test input";
            string completionResult = "completed sentence";
            _openAIServiceMock.Setup(x => x.CompleteSentence(input)).ReturnsAsync(completionResult);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.ExplainTask(input);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal(completionResult, contentResult.Content);
            Assert.Equal("text/plain", contentResult.ContentType);
        }

        [Fact]
        public async Task Delete_ReturnsViewResult_WithNoteWhenNoteExists()
        {
            // Arrange
            int noteId = 1;
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = noteId,
            };
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ReturnsAsync(note);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Delete(noteId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(note, viewResult.Model);
        }

        [Fact]
        public async Task Delete_ReturnsNotFoundResult_WhenNoteDoesNotExist()
        {
            // Arrange
            int noteId = 1;
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ReturnsAsync((Note)null);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Delete(noteId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsRedirectToIndex_WhenNoteAccessDeniedExceptionIsThrown()
        {
            // Arrange
            int noteId = 1;
            _notesServiceMock.Setup(x => x.GetNoteAsync(noteId)).ThrowsAsync(new NoteAccessDeniedException("1234567", noteId, "Delete"));

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Delete(noteId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public void Error_ReturnsViewResultWithModelErrorViewModel()
        {
            // Arrange
            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Set up a ControllerContext with a HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task Edit_Post_RedirectsToIndex_WhenModelStateIsValidAndNoteIsUpdated()
        {
            // Arrange
            var noteId = 1;
            var editNoteRequest = new EditNoteRequest
            {
                Id = noteId,
                Title = "Updated Title",
                Contents = "Updated Contents",
                Status = NoteStatus.Public,
                AvgRating = 4.5f,
                UserId = "1234567"
            };

            _notesServiceMock.Setup(x => x.UpdateNoteAsync(editNoteRequest)).ReturnsAsync(true);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Edit(editNoteRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_WhenModelStateIsValidButNoteUpdateFails()
        {
            // Arrange
            var noteId = 1;
            var editNoteRequest = new EditNoteRequest
            {
                Id = noteId,
                Title = "Updated Title",
                Contents = "Updated Contents",
                Status = NoteStatus.Public,
                AvgRating = 4.5f,
                UserId = "1234567"
            };

            _notesServiceMock.Setup(x => x.UpdateNoteAsync(editNoteRequest)).ReturnsAsync(false);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Edit(editNoteRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidModel = new EditNoteRequest(); // Invalid

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);
            controller.ModelState.AddModelError("Title", "Title is required"); // Simulate ModelState.IsValid being false

            // Act
            var result = await controller.Edit(invalidModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(invalidModel, viewResult.Model);
        }


        [Fact]
        public async Task DeleteConfirmed_Post_RedirectsToIndex_WhenNoteIsDeletedSuccessfully()
        {
            // Arrange
            var noteId = 1;
            var deleteNoteRequest = new DeleteNoteRequest
            {
                Id = noteId
            };

            _notesServiceMock.Setup(x => x.DeleteNoteAsync(deleteNoteRequest)).ReturnsAsync(true);

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.DeleteConfirmed(deleteNoteRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_RedirectsToIndex_WhenModelStateIsValidAndNoteIsCreated()
        {
            // Arrange
            var createNoteRequest = new CreateNoteRequest
            {
                Title = "Test Title",
                Contents = "Test Contents",
                Status = NoteStatus.Public,
                UserId = "1234567"
            };

            _notesServiceMock.Setup(x => x.CreateNoteAsync(createNoteRequest)).ReturnsAsync(1); // Simulate the ID of the created note

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);

            // Act
            var result = await controller.Create(createNoteRequest);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            var invalidModel = new CreateNoteRequest(); // Invalid

            var controller = new NotesController(_openAIServiceMock.Object, _notesServiceMock.Object, _ratingServiceMock.Object, _userManagerMock.Object, _loggerMock.Object);
            controller.ModelState.AddModelError("Title", "Title is required"); // Simulate ModelState.IsValid being false

            // Act
            var result = await controller.Create(invalidModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(invalidModel, viewResult.Model);
        }
    }
}
