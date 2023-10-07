namespace OnlineNotes.Models
{
    public class Comment
    {
        public int Id  { get; set; }
        public string Contents { get; set; }
        public DateTime CreationDate { get; set; }

        // Foreign key to associate with a Note
        public int NoteId { get; set; }
         //Required reference navigation to principal
        public Note Note { get; set; } = null!;
    }
}
