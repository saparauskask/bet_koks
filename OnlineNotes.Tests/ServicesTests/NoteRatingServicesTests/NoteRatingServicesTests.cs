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

namespace OnlineNotes.Tests.ServicesTests.NoteRatingServicesTests
{
    public class NoteRatingServicesTests
    {

        [Fact]
        public async Task AddOrUpdateNoteRatingAsync_CreateRating_ReturnsTrue()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NoteRatingService>>();
            var mockRefRep = new Mock<ReferencesRepository>(context, A.Fake<IHttpContextAccessor>());
            var service = new NoteRatingService(mockRefRep.Object, mockLogger.Object);

            var note = new Note("Title", "Contents", NoteStatus.Public);

            note.Id = 1;
            note.UserId = "abc123";
            note.AvgRating = null;
            note.CreationDate = DateTime.Now;

            

            context.Note.Add(note);
            context.SaveChanges();

            var userId = "testUser";
            var rating = 5;

            // Act
            var result = await service.AddOrUpdateNoteRatingAsync(userId, note, null, rating);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateNoteRatingAsync_Success_ReturnsTrue()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NoteRatingService>>();
            var mockRefRep = new Mock<ReferencesRepository>(context, A.Fake<IHttpContextAccessor>());
            var service = new NoteRatingService(mockRefRep.Object, mockLogger.Object);

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
            var result = await service.CreateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateNoteRatingAsync_ValidRequest_ReturnsTrue()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NoteRatingService>>();
            var mockRefRep = new Mock<ReferencesRepository>(context, A.Fake<IHttpContextAccessor>());
            var service = new NoteRatingService(mockRefRep.Object, mockLogger.Object);

            var noteRatingRequest = new EditNoteRatingRequest
            {
                Id = 1,
                RatingValue = 5,
                CreationDate = DateTime.Now
            };

            context.NoteRating.Add(new NoteRating
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
            context.SaveChanges();

            // Act
            var result = await service.UpdateNoteRatingAsync(noteRatingRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetNoteRatingAsync_WithValidId_ReturnsNoteRating()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NoteRatingService>>();
            var mockRefRep = new Mock<ReferencesRepository>(context, A.Fake<IHttpContextAccessor>());
            var service = new NoteRatingService(mockRefRep.Object, mockLogger.Object);

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

            context.NoteRating.Add(noteRating);
            context.SaveChanges();

            // Act
            var result = await service.GetNoteRatingAsync(noteRating.Id);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetNoteRatingAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NoteRatingService>>();
            var mockRefRep = new Mock<ReferencesRepository>(context, A.Fake<IHttpContextAccessor>());
            var service = new NoteRatingService(mockRefRep.Object, mockLogger.Object);

            // Act
            var result = await service.GetNoteRatingAsync(1);

            // Assert
            Assert.Null(result);
        }
    }
}
