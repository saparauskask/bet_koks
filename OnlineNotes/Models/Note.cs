using OnlineNotes.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineNotes.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Title { get; set; }
        public string Contents { get; set; }
        // Collection navigation containing dependents
        public ICollection<Comment> Comments { get; } = new List<Comment>();
        public NoteStatus Status { get; set; }
        public ICollection<NoteRating> Ratings { get; } = new List<NoteRating>();
        public float? AvgRating { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreationDate { get; set; }

        public Note(string title, string contents, NoteStatus status)
        {
            Title = title;
            Contents = contents;
            Status = status;
        }
    }
}
