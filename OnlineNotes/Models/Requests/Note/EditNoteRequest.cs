using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models.Requests.Note
{
    public class EditNoteRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public NoteStatus Status { get; set; }
        public string Contents { get; set; }
        public float? AvgRating { get; set; }
    }
}
