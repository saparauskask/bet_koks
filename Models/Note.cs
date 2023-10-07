namespace OnlineNotes.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        // Collection navigation containing dependents
        public ICollection<Comment> Comments { get; } = new List<Comment>();
    }
}
