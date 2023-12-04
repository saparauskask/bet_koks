using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineNotes.Models
{
    public class NoteAttachment
    {
        public int Id { get; set; }
        // Foreign key to associate with a Note
        public int NoteId { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        public string FilePath { get; set; }
        // Navigation property
        public Note Note { get; set; } = null!;
    }
}
