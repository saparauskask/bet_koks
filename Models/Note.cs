using Microsoft.Extensions.FileProviders.Composite;
using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        // Collection navigation containing dependents
        public ICollection<Comment> Comments { get; } = new List<Comment>();
        public NoteStatus Status { get; set; }
        public NoteRating Rating { get; set; }

        public Note(string title, string contents, NoteStatus status)
        {
            Title = title;
            Contents = contents;
            Status = status;
            Rating = new NoteRating();
        }
    }
}
