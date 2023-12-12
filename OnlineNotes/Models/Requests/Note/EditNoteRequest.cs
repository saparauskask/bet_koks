using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models.Requests.Note
{
    public class EditNoteRequest : BaseNoteRequest
    {
        public float? AvgRating { get; set; }
    }
}
