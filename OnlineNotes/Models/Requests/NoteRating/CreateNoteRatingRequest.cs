namespace OnlineNotes.Models
{
    public class CreateNoteRatingRequest
    {
        public string UserId { get; set; } = null!;
        public int RatingValue { get; set; }
        public DateTime CreationDate { get; set; }
        public int NoteId { get; set; }
        public Note Note { get; set; } = null!;
    }
}
