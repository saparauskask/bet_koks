using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Exceptions;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Pagination;
using OnlineNotes.Models.Requests.Note;
using System.Security.Claims;

namespace OnlineNotes.Services.NotesServices
{
    public class NotesService : INotesService
    {
        private readonly ReferencesRepository _referencesRepository;
        private readonly ILogger<NotesService> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public NotesService(ReferencesRepository referencesRepository, ILogger<NotesService> logger, UserManager<IdentityUser> userManager)
        {
            _referencesRepository = referencesRepository;
            _logger = logger;
            _userManager = userManager;
        }

        public delegate NoteStatus GetNoteStatusFromString(EditNoteRequest note, ApplicationDbContext context);

        public NoteStatus? GetFilterStatus()
        {
            if (_referencesRepository.httpContextAccessor.HttpContext != null)
            {
                string? filterStatusString = _referencesRepository.httpContextAccessor.HttpContext.Session.GetString("FilterStatus");

                switch (filterStatusString)
                {
                    case "Public":
                        return NoteStatus.Public;
                    case "Draft":
                        return NoteStatus.Draft;
                    case "Archived":
                        return NoteStatus.Archived;
                    default:
                        return null;
                }
            }
            return null;
        }

        public IEnumerable<Note>? GetSortedNotes(int sortInt, IEnumerable<Note> notes)
        {
            try
            {
                // 1 - sort ascending, 0 - sort descending
                int? sortStatusInt = _referencesRepository.httpContextAccessor.HttpContext.Session.GetInt32("SortStatus");
                if (sortStatusInt == 0)
                {
                    return notes.OrderByDescending(i => i.CreationDate);
                }

                return notes.OrderBy(i => i.CreationDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetSortedNotes: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        public IEnumerable<Note>? GetPagedNotes(IEnumerable<Note> notes, int page, Controller controller)
        {
            if (page < 1)
            {
                page = 1;
            }

            int recsCount = notes.Count();
            var pager = new Pager(recsCount, page);
            int recSkip = (page - 1) * (int)PaginationSettings.DefaultPageSize;

            var data = notes.Skip(recSkip).Take(pager.PageSize).ToList();

            controller.ViewBag.Pager = pager;

            return data;
        }

        public int? SetSortStatus(int sortStatus)
        {
            if (_referencesRepository.httpContextAccessor.HttpContext != null)
            {
                _referencesRepository.httpContextAccessor.HttpContext.Session.SetInt32("SortStatus", sortStatus);
                return sortStatus;
            }
            return null;
        }
        public int GetSortStatus()
        {
            if (_referencesRepository.httpContextAccessor.HttpContext != null)
            {
                int? sortStatus = _referencesRepository.httpContextAccessor.HttpContext.Session.GetInt32("SortStatus");
                if (sortStatus.HasValue)
                {
                    return sortStatus.Value;
                }
            }
            return 1;
        }

        public async Task<int> CreateNoteAsync(CreateNoteRequest noteRequest)
        {
            try
            {
                Note note = new(noteRequest.Title, noteRequest.Contents, noteRequest.Status)
                {
                    UserId = noteRequest.UserId
                };

                await _referencesRepository.applicationDbContext.Note.AddAsync(note);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();

                return note.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the note: {ExceptionMessage}.", ex.Message);
                return -1;
            }
        }

        public async Task<bool> DeleteNoteAsync(DeleteNoteRequest noteRequest)
        {
            try
            {
                Task<Note?> note = _referencesRepository.applicationDbContext.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .FirstOrDefaultAsync(m => m.Id == noteRequest.Id);

                if (note.Result == null)
                {
                    return false;
                }

                foreach (var comment in note.Result.Comments.ToList())
                {
                    _referencesRepository.applicationDbContext.Comment.Remove(comment);
                }

                _referencesRepository.applicationDbContext.Note.Remove(note.Result);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the note: {ExceptionMessage}", ex.Message);
                return false;
            }
        }

        public async Task<Note?> GetNoteAsync(int? id)
        {
            ClaimsPrincipal? user = _referencesRepository.httpContextAccessor.HttpContext?.User;

            if (id == null || user == null)
            {
                return null;
            }

            IdentityUser currUser = await _userManager.GetUserAsync(user);
            var userId = currUser.Id;

            var note = await _referencesRepository.applicationDbContext.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .Include(n => n.Ratings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (note != null && string.IsNullOrEmpty(note.UserId)) // temporary fix if UserId was not set previously (there was no UserId property on the Note model before)
            {
                note.UserId = userId;
            }

            if (note != null && note.Status == NoteStatus.Draft && note.UserId != userId)
            {
                throw new NoteAccessDeniedException(userId, note.Id, "\"operation\""); // will finish implementing later
            }
            return note;
        }

        public async Task<IEnumerable<Note>?> GetFilteredNotesToListAsync(NoteStatus? filterStatus, string currentUserId)
        {
            try
            {
                if (_referencesRepository.httpContextAccessor.HttpContext != null)
                {
                    _referencesRepository.httpContextAccessor.HttpContext.Session.SetString("FilterStatus", filterStatus.ToString());
                }

                if (filterStatus.HasValue)
                {
                    if (filterStatus == NoteStatus.Draft)
                    {
                        var notes = await _referencesRepository.applicationDbContext.Note
                            .Where(note => note.Status == NoteStatus.Draft && note.UserId == currentUserId)
                            .ToListAsync();
                        return notes.AsEnumerable();
                    }
                    else
                    {
                        var notes = await _referencesRepository.applicationDbContext.Note
                            .Where(note => note.Status == filterStatus)
                            .ToListAsync();
                        return notes.AsEnumerable();
                    }

                }
                else
                {
                    var notes = await _referencesRepository.applicationDbContext.Note
                        .Where(note => (note.Status == NoteStatus.Public) || (note.Status == NoteStatus.Archived) || (note.Status == NoteStatus.Draft && note.UserId == currentUserId))
                        .ToListAsync();
                    return notes.AsEnumerable();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetFilteredNotesToListAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        public string? SetFilterStatus(NoteStatus? filterStatus)
        {
            if (_referencesRepository.httpContextAccessor.HttpContext != null)
            {
                _referencesRepository.httpContextAccessor.HttpContext.Session.SetString("FilterStatus", filterStatus.ToString());
                return filterStatus.ToString();
            }
            return null;
        }

        public async Task<IEnumerable<Note>?> GetIndexedNotesToListAsync(string term)
        {
            try
            {
                // Makes search term and Note title lowercase to make searching case insensitive
                string lowerTerm = term.ToLower();
                var notes = await _referencesRepository.applicationDbContext.Note.Where(note => note.Title.ToLower().Contains(lowerTerm)).ToListAsync();
                return notes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetIndexedNotesToListAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateNoteAsync(EditNoteRequest noteRequest)
        {
            try
            {
                // Retrieve the existing note
                Note note = await _referencesRepository.applicationDbContext.Note.FindAsync(noteRequest.Id);

                if (note == null)
                {
                    return false;
                }

                // Update the note properties
                note.Title = noteRequest.Title;
                note.Contents = noteRequest.Contents;
                note.Status = noteRequest.Status;
                note.AvgRating = noteRequest.AvgRating;
                note.UserId = noteRequest.UserId;

                _referencesRepository.applicationDbContext.Update(note);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Note with ID: {NoteId}", noteRequest.Id);
                return false;
            }
        }

        public async Task<bool> CalculateAvgRating(Note? note)
        {
            try
            {
                if (note == null) { return false; }

                note = await _referencesRepository.applicationDbContext.Note
                    .Include(n => n.Comments) // Include the Comments navigation property
                    .Include(n => n.Ratings)
                    .FirstOrDefaultAsync(m => m.Id == note.Id);

                if (note == null || note.Ratings == null) return false;

                float totalRating = 0;
                foreach (var rating in note.Ratings)
                {
                    totalRating += rating.RatingValue;
                }

                float averageRating = (float)Math.Round(totalRating / note.Ratings.Count, 2);

                note.AvgRating = averageRating;
                _referencesRepository.applicationDbContext.Update(note);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Note with ID: {NoteId}", note.Id);
                return false;
            }
        }

        public int? GetNoteRatingIdByUserId(Note note, string userId)
        {
            try
            {
                var noteRatingId = note.Ratings
                    .Where(nr => nr.UserId == userId)
                    .Select(nr => (int?)nr.Id) // Project Id or null if not found
                    .FirstOrDefault();
                return noteRatingId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetNoteRatingIdByUserId: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        // get notesId and titles
        public async Task<IEnumerable<Note>?> GetAllNotesToListAsync()
        {
            try
            {
                ClaimsPrincipal? user = _referencesRepository.httpContextAccessor.HttpContext?.User;
                if (user == null)
                {
                    return null;
                }

                IdentityUser currUser = await _userManager.GetUserAsync(user);
                var currentUserId = currUser.Id;
                var notes = await _referencesRepository.applicationDbContext.Note
                    .Where(note => note.UserId == currentUserId)
                    .ToListAsync();

                return notes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllNotesTitlesToListAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        }
    }
}