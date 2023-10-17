using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;

namespace OnlineNotes.Services.NotesServices
{
    public interface INotesService
    {
        Task<List<Note>?> GetNotesToListAsync();
        Task<Note?> GetNoteAsync(int? id);
        Task<bool> CreateNoteAsync(Note note);
        Task<bool> UpdateNoteAsync(int id, Note note);
        Task<bool> DeleteNoteAsync(Note note);
    }
}
