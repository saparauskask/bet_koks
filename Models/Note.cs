namespace OnlineNotes.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
