using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models;
using OnlineNotes.Services.NotesServices;

namespace OnlineNotes.Tests.ServicesTests.NotesServicesTests
{
    public class NoteRatingTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<NotesService>> _mockLogger;
        private readonly Mock<ReferencesRepository> _mockRefRep;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly NotesService _service;

        public NoteRatingTests()
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
        public async Task NotesService_CalculateAvgRating_WhenNoteExistsAndHasRatings_ReturnsTrue()
        {
            // Arrange
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now,
            };

            _context.Note.Add(note);

            NoteRating noteRating1 = new()
            {
                UserId = note.UserId,
                RatingValue = 1,
                CreationDate = DateTime.Now,
                Note = note
            };
            NoteRating noteRating2 = new()
            {
                UserId = note.UserId,
                RatingValue = 4,
                CreationDate = DateTime.Now,
                Note = note
            };

            _context.NoteRating.AddRange(noteRating1, noteRating2);
            _context.SaveChanges();

            // Act
            var result = await _service.CalculateAvgRating(note);

            // Assert
            Assert.True(result);
            Assert.Equal(2.5f, note.AvgRating);
        }

        [Fact]
        public async Task NotesService_CalculateAvgRating_WhenNoteIsNull_ReturnsFalse()
        {
            // Act
            var result = await _service.CalculateAvgRating(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NotesService_CalculateAvgRating_WhenNoteRatingDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var note = new Note("Test title", "Test content", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            // Act
            var result = await _service.CalculateAvgRating(note);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task NotesService_CalculateAvgRating_WithInvalidRefRep_ReturnsFalse()
        {
            // Arrange
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now,
            };

            _context.Note.Add(note);

            NoteRating noteRating1 = new()
            {
                UserId = note.UserId,
                RatingValue = 1,
                CreationDate = DateTime.Now,
                Note = note
            };
            NoteRating noteRating2 = new()
            {
                UserId = note.UserId,
                RatingValue = 4,
                CreationDate = DateTime.Now,
                Note = note
            };

            _context.NoteRating.AddRange(noteRating1, noteRating2);
            _context.SaveChanges();

            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, null);
            var invalidService = new NotesService(mockInvalidRefRep.Object, _mockLogger.Object, _mockUserManager.Object);

            // Act
            var result = await invalidService.CalculateAvgRating(note);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void NotesService_GetNoteRatingIdByUserId_WhenValidNoteAndHasRatings_ReturnsNoteRatingId()
        {
            // Arrange
            var note = new Note("Test title", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now,
            };

            _context.Note.Add(note);

            NoteRating noteRating1 = new()
            {
                Id = 1,
                UserId = note.UserId,
                RatingValue = 1,
                CreationDate = DateTime.Now,
                Note = note
            };

            _context.NoteRating.Add(noteRating1);
            _context.SaveChanges();

            // Act
            var result = _service.GetNoteRatingIdByUserId(note, "1234567");

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void NotesService_GetNoteRatingIdByUserId_WhenNoteIsNull_ReturnsNull()
        {
            // Act
            var result = _service.GetNoteRatingIdByUserId(null, "1234567");

            // Assert
            Assert.Null(result);
        }
    }
}
