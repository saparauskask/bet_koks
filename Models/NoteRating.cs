using System.ComponentModel.DataAnnotations;

namespace OnlineNotes.Models
{
    public class NoteRating
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        [Range (1,5)]
        public int RatingValue { get; set; }
        public DateTime CreationDate { get; set; }
        //Foreign key
        public int NoteId { get; set; }
        //Navigation property
        public Note Note { get; set; } = null!;
        
    }
}
