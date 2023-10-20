using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models.Requests
{
    public class CreateNoteRequest
    {
        public string Title { get; private set; }
        public NoteStatus Status { get; set; }
    }
}
