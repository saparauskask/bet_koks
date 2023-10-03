namespace OnlineNotes.Models
{
    public class Comment
    {
        // Foreign key to associate with a Note
        public int? NoteId { get; set; } // foreign key property
        //public Note Note { get; set; } // navigation property


        public int? Id  { get; set; }
        public string? Contents { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
