using OnlineNotes.Models;
using OnlineNotes.Models.Requests.NoteRating;

namespace OnlineNotes.Services.RatingServices
{
    public interface INoteRatingService
    {
        public Task<bool> CreateNoteRatingAsync(CreateNoteRatingRequest noteRatingRequest);
        public Task<bool> UpdateNoteRatingAsync(EditNoteRatingRequest noteRatingRequest);
        public Task<NoteRating?> GetNoteRatingAsync(int? id);
    }
}
