namespace OnlineNotes.Models.Interfaces
{
    public interface IAuditable
    {
        DateTime CreationDate { get; set; }
        DateTime? ModificationDate { get; set; }
    }
}
