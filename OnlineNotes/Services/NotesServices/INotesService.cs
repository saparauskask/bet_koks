using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;

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
        int GetSortStatus();
        IEnumerable<Note>? GetSortedNotes(int sortInt, IEnumerable<Note> notes);
        IEnumerable<Note>? GetPagedNotes(IEnumerable<Note> notes, int page, Controller controller);
        string? SetFilterStatus(NoteStatus? filterStatus);
        int? GetNoteRatingIdByUserId(Note note, string userId);
        Task<IEnumerable<Note>?> GetAllNotesToListAsync();
    }
}
