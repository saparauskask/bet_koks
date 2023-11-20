using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models.Requests.Note
{
    public class EditNoteRequest : BaseNoteRequest
    {
        public string Title { get; set; }
        public NoteStatus Status { get; set; }
        public string Contents { get; set; }
    }
}
