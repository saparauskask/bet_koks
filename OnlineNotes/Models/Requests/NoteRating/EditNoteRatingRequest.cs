namespace OnlineNotes.Models.Requests.NoteRating
{
    public class EditNoteRatingRequest
    {
        public int Id { get; set; }
        public int RatingValue { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
