using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Exceptions;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;
using OnlineNotes.Services.NotesServices;
using System.Security.Claims;

namespace OnlineNotes.Tests.ServicesTests.NotesServicesTests
{
    public class CRUDTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<NotesService>> _mockLogger;
        private readonly Mock<ReferencesRepository> _mockRefRep;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly NotesService _service;

        public CRUDTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<NotesService>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);

            _mockRefRep = new Mock<ReferencesRepository>(_context, A.Fake<IHttpContextAccessor>());
            _service = new NotesService(_mockRefRep.Object, _mockLogger.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task NotesService_CreateNoteAsync_WithValidNoteRequest_ReturnsTrue()
        {
            // Arrange
            var noteRequest = new CreateNoteRequest
            {
                Title = "Test Note",
                Contents = "Test Content",
                Status = NoteStatus.Public,
                UserId = "1234567"
            };

            // Act
            var result = await _service.CreateNoteAsync(noteRequest);

            // Assert
            Assert.True(result > 0);
            Assert.Single(_context.Note);
        }

        [Fact]
        public async Task NotesService_CreateNoteAsync_WithInvalidNoteRequest_ReturnsNegativeOne()
        {
            // Arrange
            var noteRequest = new CreateNoteRequest
            {
                // Invalid note request
                Title = null,
                Contents = null,
                Status = NoteStatus.Public,
                UserId = "1234567"
            };

            // Act
            var result = await _service.CreateNoteAsync(noteRequest);

            // Assert
            Assert.Equal(-1, result);
            Assert.Empty(_context.Note);
        }

        [Fact]
        public async Task NotesService_DeleteNoteAsync_WithValidNoteRequest_ReturnsTrue()
        {
            // Arrange
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567"
            };

            _context.Note.Add(note);

            // Adds comments to the database
            var comment1 = new Comment
            {
                Contents = "Test Comment 1",
                NoteId = 1,
                CreationDate = DateTime.Now
            };
            var comment2 = new Comment
            {
                Contents = "Test Comment 2",
                NoteId = 1,
                CreationDate = DateTime.Now
            };

            _context.Comment.AddRange(comment1, comment2);
            _context.SaveChanges();

            var noteRequest = new DeleteNoteRequest
            {
                Id = 1
            };

            // Act
            var result = await _service.DeleteNoteAsync(noteRequest);

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Comment);
        }

        [Fact]
        public async Task NotesService_DeleteNoteAsync_WithInvalidNoteRequest_ReturnsFalse()
        {
            // Arrange
            var noteRequest = new DeleteNoteRequest
            {
                Id = 0
            };

            // Act
            var result = await _service.DeleteNoteAsync(noteRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NotesService_DeleteNoteAsync_WithInvalidApplicationDbContext_ReturnsFalse()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, A.Fake<IHttpContextAccessor>());
            var invalidService = new NotesService(mockInvalidRefRep.Object, _mockLogger.Object, _mockUserManager.Object);

            var noteRequest = new DeleteNoteRequest
            {
                Id = 0
            };

            // Act
            var result = await invalidService.DeleteNoteAsync(noteRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NotesService_GetNoteAsync_WithValidId_ReturnsNote()
        {
            // Arrange
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567"
            };

            _context.Note.Add(note);
            _context.SaveChanges();

            // Set up the mock UserManager to return a mock user
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = "1234567" });

            // Act
            var result = await _service.GetNoteAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(note.Id, result.Id);
            Assert.Equal(note.Contents, result.Contents);
        }

        [Fact]
        public async Task NotesService_GetNoteAsync_WithValidIdAndEmptyUserId_ReturnsNote()
        {
            // Arrange
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = ""
            };

            _context.Note.Add(note);
            _context.SaveChanges();

            // Set up the mock UserManager to return a mock user
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = "1234567" });

            // Act
            var result = await _service.GetNoteAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(note.Id, result.Id);
            Assert.Equal(note.Contents, result.Contents);
            Assert.Equal(note.UserId, result.UserId); // GetNoteAsync should set the UserId to the current user's Id
        }

        [Fact]
        public async Task NotesService_GetNoteAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            // Set up the mock UserManager to return a mock user
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = "1234567" });

            // Act
            var result = await _service.GetNoteAsync(0);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task NotesService_GetNoteAsync_WithNullId_ReturnsNull()
        {
            // Arrange
            // Set up the mock UserManager to return a mock user
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = "1234567" });

            // Act
            var result = await _service.GetNoteAsync(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task NotesService_GetNoteAsync_WithDraftNoteAndDifferentUser_ThrowsException()
        {
            // Arrange
            // Set up the mock UserManager to return a mock user
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = "7654321" });

            // Adds note to the database
            var note = new Note("Test title", "Test contents", NoteStatus.Draft)
            {
                Id = 1,
                UserId = "1234567"
            };

            _context.Note.Add(note);
            _context.SaveChanges();

            // Act and Assert
            await Assert.ThrowsAsync<NoteAccessDeniedException>(() => _service.GetNoteAsync(1));
        }

        [Fact]
        public async Task NotesService_UpdateNoteAsync_WithValidNoteRequest_ReturnsTrue()
        {
            // Arrange
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                AvgRating = 1.0f
            };

            _context.Note.Add(note);
            _context.SaveChanges();

            var noteRequest = new EditNoteRequest
            {
                Id = 1,
                Title = "Updated Note",
                Contents = "Updated Content",
                Status = NoteStatus.Archived,
                AvgRating = note.AvgRating,
                UserId = "1234567"
            };

            // Act
            var result = await _service.UpdateNoteAsync(noteRequest);
            var updatedNote = _context.Note.Find(noteRequest.Id);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedNote);
            Assert.Equal(updatedNote.Id, noteRequest.Id);
            Assert.Equal(updatedNote.Title, noteRequest.Title);
            Assert.Equal(updatedNote.Contents, noteRequest.Contents);
            Assert.Equal(updatedNote.Status, noteRequest.Status);
            Assert.Equal(updatedNote.AvgRating, noteRequest.AvgRating);
        }

        [Fact]
        public async Task NotesService_UpdateNoteAsync_WithInvalidNoteRequest_ReturnsFalse()
        {
            // Arrange
            var noteRequest = new EditNoteRequest
            {
                Id = 0,
                Title = "Updated Note",
                Contents = "Updated Content",
                Status = NoteStatus.Public,
                AvgRating = 4.5f,
                UserId = "1234567"
            };

            // Act
            var result = await _service.UpdateNoteAsync(noteRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NotesService_UpdateNoteAsync_WithInvalidApplicationDbContext_ReturnsFalse()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, A.Fake<IHttpContextAccessor>());
            var invalidService = new NotesService(mockInvalidRefRep.Object, _mockLogger.Object, _mockUserManager.Object);

            var noteRequest = new EditNoteRequest
            {
                Id = 0,
                Title = "Updated Note",
                Contents = "Updated Content",
                Status = NoteStatus.Public,
                AvgRating = 4.5f,
                UserId = "1234567"
            };

            // Act
            var result = await invalidService.UpdateNoteAsync(noteRequest);

            // Assert
            Assert.False(result);
        }
    }
}
