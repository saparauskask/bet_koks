using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Models.Requests.NoteRating;
using OnlineNotes.Models;
using OnlineNotes.Services.RatingServices;
using OnlineNotes.Models.Enums;
using Microsoft.AspNetCore.Identity;
using OnlineNotes.Services.NotesServices;

namespace OnlineNotes.Tests.ServicesTests.NoteRatingServicesTests
{
    public class NoteRatingServicesTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<NoteRatingService>> _mockLogger;
        private readonly Mock<ReferencesRepository> _mockRefRep;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly NoteRatingService _service;

        public NoteRatingServicesTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _mockLogger = new Mock<ILogger<NoteRatingService>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(new Mock<IUserStore<IdentityUser>>().Object, null, null, null, null, null, null, null, null);

            _mockRefRep = new Mock<ReferencesRepository>(_context, A.Fake<IHttpContextAccessor>());
            _service = new NoteRatingService(_mockRefRep.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddOrUpdateNoteRatingAsync_CreateRating_ReturnsTrue()
        {
            // Arrange
            var note = new Note("Title", "Contents", NoteStatus.Public);

            note.Id = 1;
            note.UserId = "abc123";
            note.AvgRating = null;
            note.CreationDate = DateTime.Now;

            _context.Note.Add(note);
            _context.SaveChanges();

            var userId = "testUser";
            var rating = 5;

            // Act
            var result = await _service.AddOrUpdateNoteRatingAsync(userId, note, null, rating);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AddOrUpdateNoteRatingAsync_UpdateRating_ReturnsTrue()
        {
            // Arrange
            var note = new Note("Title", "Contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "abc123",
                AvgRating = null,
                CreationDate = DateTime.Now
            };

            _context.Note.Add(note);

            var noteRating = new NoteRating
            {
                Id = 1,
                UserId = "abc123",
                RatingValue = 1,
                CreationDate = DateTime.Now,
                NoteId = 1,
                Note = note
            };

            _context.NoteRating.Add(noteRating);
            _context.SaveChanges();

            var userId = "abc123";
            var newRating = 5;

            // Act
            var result = await _service.AddOrUpdateNoteRatingAsync(userId, note, 1, newRating);

            // Assert
            Assert.True(result);
            var updatedNoteRating = _context.NoteRating.Find(1);
            Assert.NotNull(updatedNoteRating);
            Assert.Equal(5, updatedNoteRating.RatingValue);
        }

        [Fact]
        public async Task CreateNoteRatingAsync_Success_ReturnsTrue()
        {
            // Arrange
            var noteRatingRequest = new CreateNoteRatingRequest
            {
                UserId = "testUser",
                RatingValue = 5,
                CreationDate = DateTime.Now,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = null,
                    CreationDate = DateTime.Now
                }
            };

            // Act
            var result = await _service.CreateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.True(result);
        }


        [Fact]
        public async Task CreateNoteRatingAsync_InvalidRefRep_ReturnsFalse()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, A.Fake<IHttpContextAccessor>());
            var invalidService = new NoteRatingService(mockInvalidRefRep.Object, _mockLogger.Object);

            var noteRatingRequest = new CreateNoteRatingRequest
            {
                UserId = "testUser",
                RatingValue = 5,
                CreationDate = DateTime.Now,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = null,
                    CreationDate = DateTime.Now
                }
            };

            // Act
            var result = await invalidService.CreateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateNoteRatingAsync_ValidRequest_ReturnsTrue()
        {
            // Arrange
            var noteRatingRequest = new EditNoteRatingRequest
            {
                Id = 1,
                RatingValue = 5,
                CreationDate = DateTime.Now
            };

            _context.NoteRating.Add(new NoteRating
            {
                Id = 1,
                UserId = "abc123",
                RatingValue = 5,
                CreationDate = DateTime.Now,
                NoteId = 1,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = 5,
                    CreationDate = DateTime.Now
                }
            });
            _context.SaveChanges();

            // Act
            var result = await _service.UpdateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateNoteRatingAsync_InvalidNoteRatingRequestId_ReturnsFalse()
        {
            // Arrange
            var noteRatingRequest = new EditNoteRatingRequest
            {
                Id = 0,
                RatingValue = 5,
                CreationDate = DateTime.Now
            };

            var noteRating = new NoteRating()
            {
                Id = 1,
                UserId = "abc123",
                RatingValue = 5,
                CreationDate = DateTime.Now,
                NoteId = 1,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = 5,
                    CreationDate = DateTime.Now
                }
            };

            _context.NoteRating.Add(noteRating);
            _context.SaveChanges();

            // Act
            var result = await _service.UpdateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateNoteRatingAsync_NoteWithRequestIdDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var noteRatingRequest = new EditNoteRatingRequest
            {
                Id = 2,
                RatingValue = 5,
                CreationDate = DateTime.Now
            };

            var noteRating = new NoteRating()
            {
                Id = 1,
                UserId = "abc123",
                RatingValue = 5,
                CreationDate = DateTime.Now,
                NoteId = 1,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = 5,
                    CreationDate = DateTime.Now
                }
            };

            _context.NoteRating.Add(noteRating);
            _context.SaveChanges();

            // Act
            var result = await _service.UpdateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task UpdateNoteRatingAsync_InvalidRefRep_ReturnsFalse()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, null);
            var invalidService = new NoteRatingService(mockInvalidRefRep.Object, _mockLogger.Object);

            var noteRatingRequest = new EditNoteRatingRequest
            {
                Id = 1,
                RatingValue = 5,
                CreationDate = DateTime.Now
            };

            var noteRating = new NoteRating()
            {
                Id = 1,
                UserId = "abc123",
                RatingValue = 5,
                CreationDate = DateTime.Now,
                NoteId = 1,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = 5,
                    CreationDate = DateTime.Now
                }
            };

            _context.NoteRating.Add(noteRating);
            _context.SaveChanges();

            // Act
            var result = await invalidService.UpdateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetNoteRatingAsync_WithValidId_ReturnsNoteRating()
        {
            // Arrange
            var noteRating = new NoteRating
            {
                Id = 1,
                UserId = "abc123",
                RatingValue= 5,
                CreationDate= DateTime.Now,
                NoteId= 1,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = 5,
                    CreationDate = DateTime.Now
                }
            };

            _context.NoteRating.Add(noteRating);
            _context.SaveChanges();

            // Act
            var result = await _service.GetNoteRatingAsync(noteRating.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetNoteRatingAsync_InvalidRefRep_ReturnsNull()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, A.Fake<IHttpContextAccessor>());
            var invalidService = new NoteRatingService(mockInvalidRefRep.Object, _mockLogger.Object);

            var noteRating = new NoteRating
            {
                Id = 1,
                UserId = "abc123",
                RatingValue = 5,
                CreationDate = DateTime.Now,
                NoteId = 1,
                Note = new Note("Title", "Contents", NoteStatus.Public)
                {
                    Id = 1,
                    UserId = "abc123",
                    AvgRating = 5,
                    CreationDate = DateTime.Now
                }
            };

            _context.NoteRating.Add(noteRating);
            _context.SaveChanges();

            // Act
            var result = await invalidService.GetNoteRatingAsync(noteRating.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetNoteRatingAsync_WithInvalidId_ReturnsNull()
        {
            // Act
            var resultNull = await _service.GetNoteRatingAsync(null);
            var resultInvalid = await _service.GetNoteRatingAsync(1);

            // Assert
            Assert.Null(resultNull);
            Assert.Null(resultInvalid);
        }
    }
}
