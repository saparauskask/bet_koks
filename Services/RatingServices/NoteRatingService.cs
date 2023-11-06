using OnlineNotes.Data;
using OnlineNotes.Models;

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
            NoteRating noteRating = new NoteRating();
            noteRating.UserId = noteRatingRequest.UserId;
            noteRating.RatingValue = noteRatingRequest.RatingValue;
            noteRating.CreationDate = noteRatingRequest.CreationDate;
            noteRating.Note = noteRatingRequest.Note;

            try
            {
                _context.NoteRating.Add(noteRating);
                await _context.SaveChangesAsync();
                _logger.LogInformation("That's great. You have created your first rating successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the rating: {ExceptionMessage}.", ex.Message);
                return false;
            }
        }
    }
}
