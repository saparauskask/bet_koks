namespace OnlineNotes.Exceptions
{
    public class NoteAccessDeniedException : Exception
    {
        public string UserId { get; }
        public int NoteId { get; }
        public string Operation { get; }

        public NoteAccessDeniedException(string userId, int noteId, string operation)
            : base($"Access to the note (ID: {noteId}) is denied for the user (ID: {userId}) during {operation} operation.")
        {
            UserId = userId;
            NoteId = noteId;
            Operation = operation;
        }

        public string GetErrorMessage()
        {
            return $"Access to the note was denied! You do not have permission to {Operation} this note.";
        }
    }
}
