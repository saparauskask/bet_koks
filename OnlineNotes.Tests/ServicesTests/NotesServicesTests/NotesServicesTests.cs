using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Services.NotesServices;

namespace OnlineNotes.Tests
{
    public class NotesServiceTests
    {
        [Fact]
        public async void GetNoteAsync_ReturnsNote()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var service = new NotesService(context, null, null);

            var note = new Note("Test Note", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            context.Note.Add(note);
            context.SaveChanges();

            // Act
            var result = await service.GetNoteAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Note", result.Title);
        }

        [Fact]
        public void GetFilteredNotesToListAsync_ReturnsFilteredNotes()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NotesService>>();

            var service = new NotesService(context, null, mockLogger.Object);

            var note1 = new Note("Public Note", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            var note2 = new Note("Draft Note", "Test contents", NoteStatus.Draft)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            var note3 = new Note("Archived Note", "Test contents", NoteStatus.Archived)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            context.Note.AddRange(note1, note2, note3);
            context.SaveChanges();

            // Act
            var result1 = service.GetFilteredNotesToListAsync(NoteStatus.Public, "1234567");
            var result2 = service.GetFilteredNotesToListAsync(NoteStatus.Draft, "1234567");
            var result3 = service.GetFilteredNotesToListAsync(NoteStatus.Archived, "1234567");

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
        }
    }
}