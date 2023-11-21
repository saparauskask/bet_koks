using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Data.Migrations;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;
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
            Assert.Equal("Test contents", result.Contents);
            Assert.Equal("1234567", result.UserId);
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

        [Fact]
        public void CreateNoteAsync_ReturnsBool()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NotesService>>();

            var service = new NotesService(context, null, mockLogger.Object);

            CreateNoteRequest createNoteRequest = new CreateNoteRequest()
            {
                Id = 1,
                Title = "test title",
                Contents = "test contets",
                UserId = "1234567",
                Status = NoteStatus.Public
            };

            // Act
            var publicResult = service.CreateNoteAsync(createNoteRequest);
            var createdNote = context.Note.Find(publicResult);

            // Assert
            Assert.NotNull(createdNote);
            Assert.Equal("test title", createdNote.Title);
            Assert.Equal("test contets", createdNote.Contents);
            Assert.Equal("1234567", createdNote.UserId);
            Assert.Equal(NoteStatus.Public, createdNote.Status);
        }

        [Fact]
        public async Task DeleteNoteAsync_ReturnsBool()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NotesService>>();

            var service = new NotesService(context, null, mockLogger.Object);

            var note = new Note("Test note", "Test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            context.Note.Add(note);
            context.SaveChanges();

            DeleteNoteRequest deleteNoteRequest = new DeleteNoteRequest()
            {
                Id = note.Id
            };

            // Act
            var deleteResult = await service.DeleteNoteAsync(deleteNoteRequest);
            var deletedNote = context.Note.Find(note.Id);

            // Assert
            Assert.True(deleteResult);
            Assert.Null(deletedNote);
        }

        [Fact]
        public async Task GetIndexedNotesToListAsync_ReturnsNotes()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NotesService>>();

            var service = new NotesService(context, null, mockLogger.Object);

            Note note1 = new("test title", "test contents", NoteStatus.Public) 
            {
                Id = 1,
                CreationDate = DateTime.Now, 
                UserId = "1234567" 
            };
            Note note2 = new("another title", "another contents", NoteStatus.Public) 
            {
                Id = 2,
                CreationDate = DateTime.Now, 
                UserId = "1234567" 
            };
            Note note3 = new("test another title", "test another contents", NoteStatus.Public) 
            {
                Id = 3,
                CreationDate = DateTime.Now, 
                UserId = "1234567" 
            };

            context.Note.AddRange(note1, note2, note3);
            context.SaveChanges();

            string searchTerm = "test";

            // Act
            var notes = await service.GetIndexedNotesToListAsync(searchTerm);

            // Assert
            Assert.NotNull(notes);
            Assert.Equal(2, notes.Count());
            Assert.Contains(notes, note => note.Title == note1.Title);
            Assert.Contains(notes, note => note.Title == note3.Title);
        }

        [Fact]
        public async Task CalculateAvgRating_ReturnsBool()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NotesService>>();

            var service = new NotesService(context, null, mockLogger.Object);

            // Create a note and some ratings
            Note note = new("test title", "test contents", NoteStatus.Public)
            {
                Id = 1,
                CreationDate = DateTime.Now, 
                UserId = "1234567" 
            };
            context.Note.Add(note);
            context.SaveChanges();

            NoteRating rating1 = new NoteRating() 
            { 
                UserId = note.UserId,
                NoteId = note.Id, 
                RatingValue = 3 
            };
            NoteRating rating2 = new NoteRating() 
            {
                UserId = note.UserId,
                NoteId = note.Id, 
                RatingValue = 4 
            };

            context.NoteRating.AddRange(rating1, rating2);
            context.SaveChanges();

            // Act
            var updateResult = await service.CalculateAvgRating(note);

            // Assert
            Assert.True(updateResult);
            var updatedNote = context.Note.Find(note.Id);
            Assert.NotNull(updatedNote);
            Assert.Equal(3.5f, updatedNote.AvgRating);
        }

        [Fact]
        public void GetNoteRatingIdByUserId_ReturnsNoteRatingId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            var mockLogger = new Mock<ILogger<NotesService>>();

            var service = new NotesService(context, null, mockLogger.Object);

            // Create a note and a user with a rating
            Note note = new("test title", "test contents", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            context.Note.Add(note);
            context.SaveChanges();

            NoteRating rating = new NoteRating()
            {
                UserId = note.UserId,
                NoteId = note.Id,
                RatingValue = 5,
            };

            context.NoteRating.Add(rating);
            context.SaveChanges();

            // Act
            var noteRatingId = service.GetNoteRatingIdByUserId(note, "1234567");
            var invalidNoteRatingId = service.GetNoteRatingIdByUserId(note, "7654321");

            // Assert
            Assert.NotNull(noteRatingId);
            Assert.Equal(rating.Id, noteRatingId);
            Assert.Null(invalidNoteRatingId);
        }
    }
}