using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models.Requests.Note
{
    public class BaseNoteRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public NoteStatus Status { get; set; }
        public string Contents { get; set; }
        public string UserId { get; set; }
    }
}
