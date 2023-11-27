using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Requests.NoteRating;

namespace OnlineNotes.Services.RatingServices
{
    public class NoteRatingService : INoteRatingService
    {
        private readonly ReferencesRepository _refRep;
        private readonly ILogger<NoteRatingService> _logger;

        public NoteRatingService(ReferencesRepository refRep, ILogger<NoteRatingService> logger)
        {
            _refRep = refRep;
            _logger = logger;
        }

        public async Task<bool> AddOrUpdateNoteRatingAsync(string userId, Note note, int? noteRatingId, int rating)
        {
            if(noteRatingId != null)
            {
                var ratingRequest = new EditNoteRatingRequest
                {
                    Id = noteRatingId ?? 0,
                    RatingValue = rating,
                    CreationDate = DateTime.Now
                };

                return await UpdateNoteRatingAsync(ratingRequest);
            } else
            {
                var ratingRequest = new CreateNoteRatingRequest
                {
                    UserId = userId,
                    RatingValue = rating,
                    CreationDate = DateTime.Now,
                    Note = note
                };

                return await CreateNoteRatingAsync(ratingRequest);
            }
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
                _refRep.applicationDbContext.NoteRating.Add(noteRating);
                await _refRep.applicationDbContext.SaveChangesAsync();
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

                _refRep.applicationDbContext.Update(noteRating);
                await _refRep.applicationDbContext.SaveChangesAsync();
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
                var noteRating = await _refRep.applicationDbContext.NoteRating.FirstOrDefaultAsync(x => x.Id == id);
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
