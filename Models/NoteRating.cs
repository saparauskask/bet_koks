namespace OnlineNotes.Models
{
    public class NoteRating
    {
        public float AvgRating { get; set; }
        public int RatingsCount { get; set; }

        public void AddNewRating(int rating)
        {
            ++RatingsCount;
            AvgRating = (AvgRating * (RatingsCount - 1) + rating) / RatingsCount;
        }
    }
}
