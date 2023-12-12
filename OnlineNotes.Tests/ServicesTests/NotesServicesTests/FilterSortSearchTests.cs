using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Pagination;
using OnlineNotes.Services.NotesServices;
using System.Linq.Expressions;
using System.Security.Claims;

namespace OnlineNotes.Tests.ServicesTests.NotesServicesTests
{
    public class FilterSortSearchTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<NotesService>> _mockLogger;
        private readonly Mock<ReferencesRepository> _mockRefRep;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly NotesService _service;

        public FilterSortSearchTests()
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
        public void NotesService_GetSortedNotes_ReturnsSortedNotes()
        {
            // Arrange
            // Add notes to list
            var note1 = new Note("Test title 1", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
            };
            var note2 = new Note("Test title 2", "Test content 2", NoteStatus.Public)
            {
                Id = 2,
                UserId = "1234567",
            };
            var note3 = new Note("Test title 3", "Test content 3", NoteStatus.Public)
            {
                Id = 3,
                UserId = "1234567",
            };

            var notes = new List<Note> { note1, note2, note3 };

            // Act
            var resultDesc = _service.GetSortedNotes(0, notes);
            var resultAsc = _service.GetSortedNotes(1, notes);
            var resultInvalidInt = _service.GetSortedNotes(-1, notes);

            // Assert
            Assert.Equal(notes.OrderByDescending(i => i.CreationDate), resultDesc);
            Assert.Equal(notes.OrderBy(i => i.CreationDate), resultAsc);
            Assert.Equal(notes.OrderBy(i => i.CreationDate), resultInvalidInt);
        }

        [Fact]
        public void NotesService_GetSortedNotes_WithInvalidHttpContextAccessor_ReturnsNull()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(_context, null);
            var invalidService = new NotesService(mockInvalidRefRep.Object, _mockLogger.Object, _mockUserManager.Object);

            // Add notes to list
            var note1 = new Note("Test title 1", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note2 = new Note("Test title 2", "Test content 2", NoteStatus.Public)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(1)
            };
            var note3 = new Note("Test title 3", "Test content 3", NoteStatus.Public)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(2)
            };

            var notes = new List<Note> { note1, note2, note3 };

            // Act
            var resultDesc = invalidService.GetSortedNotes(0, notes);
            var resultAsc = invalidService.GetSortedNotes(1, notes);
            var resultInvalidInt = invalidService.GetSortedNotes(-1, notes);

            // Assert
            Assert.Null(resultDesc);
            Assert.Null(resultAsc);
            Assert.Null(resultInvalidInt);
        }

        [Fact]
        public async Task NotesService_GetFilteredNotesToListAsync_ReturnsCorrectlyFilteredNotes()
        {
            // Arrange
            // Add notes to list
            var note1 = new Note("Test title 1", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note2 = new Note("Test title 2", "Test content 2", NoteStatus.Draft)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(1)
            };
            var note3 = new Note("Test title 3", "Test content 3", NoteStatus.Archived)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(2)
            };

            var notes = new List<Note> { note1, note2, note3 };

            _context.Note.AddRange(note1, note2, note3);
            _context.SaveChanges();

            // Act
            var resultPublic = await _service.GetFilteredNotesToListAsync(NoteStatus.Public, "1234567");
            var resultDraft = await _service.GetFilteredNotesToListAsync(NoteStatus.Draft, "1234567");
            var resultArchived = await _service.GetFilteredNotesToListAsync(NoteStatus.Archived, "1234567");
            var resultInvalid = await _service.GetFilteredNotesToListAsync(null, "1234567");

            // Assert
            Assert.Equal(notes.Where(i => i.Status == NoteStatus.Public), resultPublic);
            Assert.Equal(notes.Where(i => i.Status == NoteStatus.Draft && i.UserId == "1234567"), resultDraft);
            Assert.Equal(notes.Where(i => i.Status == NoteStatus.Archived), resultArchived);
            Assert.Equal(notes.Where(i => (i.Status == NoteStatus.Public) || (i.Status == NoteStatus.Archived) || (i.Status == NoteStatus.Draft && i.UserId == "1234567")), resultInvalid);
        }

        [Fact]
        public async Task NotesService_GetFilteredNotesToListAsync_WithInvalidHttpContextAccessor_ReturnsNull()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(_context, null);
            var invalidService = new NotesService(mockInvalidRefRep.Object, _mockLogger.Object, _mockUserManager.Object);

            // Add notes to list
            var note1 = new Note("Test title 1", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note2 = new Note("Test title 2", "Test content 2", NoteStatus.Draft)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(1)
            };
            var note3 = new Note("Test title 3", "Test content 3", NoteStatus.Archived)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(2)
            };

            var notes = new List<Note> { note1, note2, note3 };

            _context.Note.AddRange(note1, note2, note3);
            _context.SaveChanges();

            // Act
            var resultPublic = await invalidService.GetFilteredNotesToListAsync(NoteStatus.Public, "1234567");
            var resultDraft = await invalidService.GetFilteredNotesToListAsync(NoteStatus.Draft, "1234567");
            var resultArchived = await invalidService.GetFilteredNotesToListAsync(NoteStatus.Archived, "1234567");
            var resultInvalid = await invalidService.GetFilteredNotesToListAsync(null, "1234567");

            // Assert
            Assert.Null(resultPublic);
            Assert.Null(resultDraft);
            Assert.Null(resultArchived);
            Assert.Null(resultInvalid);
        }

        [Fact]
        public async Task NotesService_GetIndexedNotesToListAsync_ReturnsCorrectlyIndexedNotes()
        {
            // Arrange
            // Add notes to list
            var note1 = new Note("Alpha", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note2 = new Note("Beta", "Test content 2", NoteStatus.Draft)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(1)
            };
            var note3 = new Note("Alpha Beta", "Test content 3", NoteStatus.Archived)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(2)
            };

            var notes = new List<Note> { note1, note2, note3 };

            _context.Note.AddRange(note1, note2, note3);
            _context.SaveChanges();

            // Act
            string searchTermAlpha = "Alpha";
            string searchTermBeta = "Beta";
            string emptySearchTerm = "";

            var alphaNotes = await _service.GetIndexedNotesToListAsync(searchTermAlpha);
            var betaNotes = await _service.GetIndexedNotesToListAsync(searchTermBeta);
            var emptyNotes = await _service.GetIndexedNotesToListAsync("");

            // Assert
            Assert.Equal(notes.Where(note => note.Title.ToLower().Contains(searchTermAlpha.ToLower())), alphaNotes);
            Assert.Equal(notes.Where(note => note.Title.ToLower().Contains(searchTermBeta.ToLower())), betaNotes);
            Assert.Equal(notes.Where(note => note.Title.ToLower().Contains(emptySearchTerm)), emptyNotes);
        }

        [Fact]
        public async Task NotesService_GetIndexedNotesToListAsync_WithInvalidApplicationDbContext_ReturnsNull()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(null, A.Fake<IHttpContextAccessor>());
            var invalidService = new NotesService(mockInvalidRefRep.Object, _mockLogger.Object, _mockUserManager.Object);

            // Add notes to list
            var note1 = new Note("Alpha", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note2 = new Note("Beta", "Test content 2", NoteStatus.Draft)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(1)
            };
            var note3 = new Note("Alpha Beta", "Test content 3", NoteStatus.Archived)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(2)
            };

            var notes = new List<Note> { note1, note2, note3 };

            _context.Note.AddRange(note1, note2, note3);
            _context.SaveChanges();

            // Act
            string searchTermAlpha = "Alpha";
            string searchTermBeta = "Beta";
            string emptySearchTerm = "";

            var alphaNotes = await invalidService.GetIndexedNotesToListAsync(searchTermAlpha);
            var betaNotes = await invalidService.GetIndexedNotesToListAsync(searchTermBeta);
            var emptyNotes = await invalidService.GetIndexedNotesToListAsync(emptySearchTerm);

            // Assert
            Assert.Null(alphaNotes);
            Assert.Null(betaNotes);
            Assert.Null(emptyNotes);
        }

        [Fact]
        public void NotesService_GetPagedNotes_ReturnsCorrectlyPagedNotes()
        {
            // Arrange
            var mockController = new Mock<Controller>();

            // Add notes to list
            var note1 = new Note("Note 1", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note2 = new Note("Note 2", "Test content 2", NoteStatus.Public)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note3 = new Note("Note 3", "Test content 3", NoteStatus.Public)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note4 = new Note("Note 4", "Test content 3", NoteStatus.Public)
            {
                Id = 4,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note5 = new Note("Note 5", "Test content 3", NoteStatus.Public)
            {
                Id = 5,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note6 = new Note("Note 5", "Test content 3", NoteStatus.Public)
            {
                Id = 5,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };

            var notes = new List<Note> { note1, note2, note3, note4, note5, note6 };

            // Act
            var resultPage1 = _service.GetPagedNotes(notes, 1, mockController.Object);
            var resultPage2 = _service.GetPagedNotes(notes, 2, mockController.Object);
            var resultInvalidPage = _service.GetPagedNotes(notes, -1, mockController.Object);

            // Assert
            Assert.Equal(notes.Take((int)PaginationSettings.DefaultPageSize), resultPage1);
            Assert.Equal(notes.Skip((int)PaginationSettings.DefaultPageSize).Take((int)PaginationSettings.DefaultPageSize), resultPage2);
            Assert.Equal(notes.Take((int)PaginationSettings.DefaultPageSize), resultInvalidPage);
        }

        [Fact]
        public async Task NotesService_GetAllNotesToListAsync_ReturnsAllNotesForCurrentUser()
        {
            // Arrange
            var note1 = new Note("Test title 1", "Test content 1", NoteStatus.Public)
            {
                Id = 1,
                UserId = "1234567",
                CreationDate = DateTime.Now
            };
            var note2 = new Note("Test title 2", "Test content 2", NoteStatus.Public)
            {
                Id = 2,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(1)
            };
            var note3 = new Note("Test title 3", "Test content 3", NoteStatus.Public)
            {
                Id = 3,
                UserId = "1234567",
                CreationDate = DateTime.Now.AddDays(2)
            };

            var notes = new List<Note> { note1, note2, note3 };

            _context.Note.AddRange(note1, note2, note3);
            _context.SaveChanges();

            // Set up the mock UserManager to return a mock user
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = "1234567" });

            // Act
            var result = await _service.GetAllNotesToListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(notes.Select(n => n.Id), result.Select(n => n.Id));
            Assert.Equal(notes.Select(n => n.Title), result.Select(n => n.Title));
            Assert.Equal(notes.Select(n => n.Contents), result.Select(n => n.Contents));
        }

        [Fact]
        public async Task NotesService_GetAllNotesToListAsync_WithInvalidHttpContextAccessor_ReturnsNull()
        {
            // Arrange
            var mockInvalidRefRep = new Mock<ReferencesRepository>(_context, null);
            var invalidService = new NotesService(mockInvalidRefRep.Object, _mockLogger.Object, _mockUserManager.Object);

            // Set up the mock UserManager to return a mock user
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new IdentityUser { Id = "1234567" });

            // Act
            var result = await invalidService.GetAllNotesToListAsync();

            // Assert
            Assert.Null(result);
        }
    }
}