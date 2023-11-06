using OnlineNotes.Models;

namespace OnlineNotes.Services.RatingServices
{
    public interface INoteRatingService
    {
        public Task<bool> CreateNoteRatingAsync(CreateNoteRatingRequest noteRatingRequest);
    }
}
