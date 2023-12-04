using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;
using static OnlineNotes.Services.NotesServices.NotesService;

namespace OnlineNotes.Services.NotesServices
{
    public interface INotesService
    {
        Task<Note?> GetNoteAsync(int? id);
        Task<int> CreateNoteAsync(CreateNoteRequest noteRequest);
        Task<bool> UpdateNoteAsync(EditNoteRequest noteRequest);
        Task<bool> DeleteNoteAsync(DeleteNoteRequest noteRequest);
        Task<IEnumerable<Note>?> GetFilteredNotesToListAsync(NoteStatus? filterStatus, string currentUserId);
        Task<IEnumerable<Note>?> GetIndexedNotesToListAsync(string term);
        NoteStatus? GetFilterStatus();
        Task<bool> CalculateAvgRating(Note note);
        int? SetSortStatus(int sortStatus);
        IEnumerable<Note>? GetSortedNotes(IEnumerable<Note> notes);
        IEnumerable<Note>? GetPagedNotes(IEnumerable<Note> notes, int page, Controller controller);
        string? SetFilterStatus(NoteStatus? filterStatus);
        int? GetNoteRatingIdByUserId(Note note, string userId);
        Task<string?> UploadFileAsync(UploadNoteAttachmentRequest request);
    }
}
