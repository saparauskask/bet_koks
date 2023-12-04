namespace OnlineNotes.Models.Requests.Note
{
    public class UploadNoteAttachmentRequest : BaseNoteRequest
    {
        public IFormFile File { get; set; }
    }
}
