namespace OnlineNotes.Models
{
    public class CreateCommentRequest
    {
        public string Contents { get; set; }
        // Foreign key to associate with a Note
        public int NoteId { get; set; }
        //Required reference navigation to principal
        public Note Note { get; set; } = null!;
    }
}
