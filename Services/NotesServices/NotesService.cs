using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Data.Migrations;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Pagination;
using OnlineNotes.Models.Requests.Note;

namespace OnlineNotes.Services.NotesServices
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<NotesService> _logger;

        public NotesService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, ILogger<NotesService> logger)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public delegate NoteStatus GetNoteStatusFromString(EditNoteRequest note, ApplicationDbContext context);

        public NoteStatus? GetFilterStatus()
        {
            if (_contextAccessor.HttpContext != null)
            {
                string? filterStatusString = _contextAccessor.HttpContext.Session.GetString("FilterStatus");

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

        public IEnumerable<Note>? GetSortedNotes(IEnumerable<Note> notes)
        {
            if (_contextAccessor.HttpContext != null)
            {
                // 1 - sort ascending, 0 - sort descending
                int? sortStatusInt = _contextAccessor.HttpContext.Session.GetInt32("SortStatus");
                if(sortStatusInt == 0)
                {
                    return notes.OrderByDescending(i => i.CreationDate);
                }

                return notes.OrderBy(i => i.CreationDate);
            }
            return null;
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
            if (_contextAccessor.HttpContext != null)
            {
                _contextAccessor.HttpContext.Session.SetInt32("SortStatus", sortStatus);
                return sortStatus;
            }
            return null;
        }

        public async Task<bool> CreateNoteAsync(CreateNoteRequest noteRequest)
        {
            try
            {
                CreateNoteDelegate(noteRequest, _context);
                _logger.LogInformation("Note with ID: {NoteId} was created successfully.", noteRequest.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the note: {ExceptionMessage}.", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteNoteAsync(DeleteNoteRequest note)
        {
            try
            {
                DeleteNoteDelegate(note, _context);
                _logger.LogInformation("Note with ID: {noteId} was deleted successfully.", note.Id);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the note: {ExceptionMessage}", ex.Message);
                return false;
            }
        }

        public async Task<Note?> GetNoteAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var note = await _context.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .Include(n => n.Ratings)
                .FirstOrDefaultAsync(m => m.Id == id);

            return note;
        }

        public async Task<IEnumerable<Note>?> GetFilteredNotesToListAsync(NoteStatus? filterStatus)
        {
            try
            {
                if (_contextAccessor.HttpContext != null)
                {
                        _contextAccessor.HttpContext.Session.SetString("FilterStatus", filterStatus.ToString());
                }

                if (filterStatus.HasValue)
                {
                    var notes = await _context.Note.Where(note => note.Status == filterStatus).ToListAsync();
                    return notes.AsEnumerable();
                }
                else
                {
                    var notes = await _context.Note.ToListAsync();
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
            if (_contextAccessor.HttpContext != null)
            {
                _contextAccessor.HttpContext.Session.SetString("FilterStatus", filterStatus.ToString());
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
                var notes = await _context.Note.Where(note => note.Title.ToLower().Contains(lowerTerm)).ToListAsync();
                return notes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetIndexedNotesToListAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        public bool UpdateNote(EditNoteRequest note)
        {
            try
            {
                EditNoteDelegate(note, _context);
                _logger.LogInformation("Note with ID: {NoteId} updated successfully.", note.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Note with ID: {NoteId}", note.Id);
                return false;
            }
        }

        // DELEGATE
        private delegate TResult UpdateNoteDelegate<T, TResult>(T noteRequest, ApplicationDbContext context)
            where T : BaseNoteRequest
            where TResult : struct;

        private UpdateNoteDelegate<EditNoteRequest, int> EditNoteDelegate = (EditNoteRequest noteReq, ApplicationDbContext context) =>
        {
            Note note = new(noteReq.Title, noteReq.Contents, noteReq.Status) { Id = noteReq.Id, CreationDate = DateTime.Now };
            context.Update(note);
            context.SaveChanges();

            // returns updated note id
            return note.Id;
        };
        private UpdateNoteDelegate<DeleteNoteRequest, bool> DeleteNoteDelegate = (DeleteNoteRequest noteReq, ApplicationDbContext context) =>
        {
            Note? note = context.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .FirstOrDefault(m => m.Id == noteReq.Id);

            if (note == null)
            {
                return false;
            }

            foreach (var comment in note.Comments.ToList())
            {
                context.Comment.Remove(comment);
            }

            context.Note.Remove(note);
            context.SaveChanges();
            return true;
        };
        
        private UpdateNoteDelegate<CreateNoteRequest, int> CreateNoteDelegate = (CreateNoteRequest noteReq, ApplicationDbContext context) =>
        {
            Note note = new(noteReq.Title, noteReq.Contents, noteReq.Status) { CreationDate = DateTime.Now };
            context.Note.Add(note);
            context.SaveChanges();
            // returns the id of the created note
            return note.Id;
        };
        
        public async Task<bool> CalculateAvgRating(Note? note)
        {
            if (note == null) { return false; }

            note = await _context.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .Include(n => n.Ratings)
                .FirstOrDefaultAsync(m => m.Id == note.Id);

            if (note == null || note.Ratings == null) return false;

            float totalRating = 0;
            foreach (var rating in note.Ratings)
            {
                totalRating += rating.RatingValue;
            }

            float averageRating =(float) Math.Round(totalRating / note.Ratings.Count, 2);

            try
            {
                note.AvgRating = averageRating;
                _context.Update(note);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Note with ID: {NoteId}", note.Id);
            }
            return false;
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
    }
}
