using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Requests.NoteRating;

namespace OnlineNotes.Services.RatingServices
{
    public class NoteRatingService : INoteRatingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NoteRatingService> _logger;

        public NoteRatingService(ApplicationDbContext context, ILogger<NoteRatingService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<bool> CreateNoteRatingAsync(CreateNoteRatingRequest noteRatingRequest)
        {
            NoteRating noteRating = new()
            {
                UserId = noteRatingRequest.UserId,
                RatingValue = noteRatingRequest.RatingValue,
                CreationDate = noteRatingRequest.CreationDate,
                Note = noteRatingRequest.Note
            };

            try
            {
                _context.NoteRating.Add(noteRating);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the rating: {ExceptionMessage}.", ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateNoteRatingAsync(EditNoteRatingRequest noteRatingRequest)
        {
            if (noteRatingRequest.Id <= 0)
            {
                return false;
            }

            var noteRating = await GetNoteRatingAsync(noteRatingRequest.Id);
            if (noteRating == null)
            {
                return false;
            }

            try
            {
                noteRating.RatingValue = noteRatingRequest.RatingValue;
                noteRating.CreationDate = noteRatingRequest.CreationDate;

                _context.Update(noteRating);
                await  _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in UpdateNoteRatingRequest: {ErrorMessage}", ex.Message);
                return false;
            }
        }

        public async Task<NoteRating?> GetNoteRatingAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            try
            {
                var noteRating = await _context.NoteRating.FirstOrDefaultAsync(x => x.Id == id);
                return noteRating;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetNoteRatingAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        }
    }
}
