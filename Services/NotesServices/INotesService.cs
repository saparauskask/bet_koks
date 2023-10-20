using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests;

namespace OnlineNotes.Services.NotesServices
{
    public interface INotesService
    {
        public IEnumerable<Note> GetNotesAsEnumerable();
        Task<List<Note>?> GetNotesToListAsync();
        Task<Note?> GetNoteAsync(int? id);
        Task<bool> CreateNoteAsync(CreateNoteRequest note);
        Task<bool> UpdateNoteAsync(int id, Note note);
        Task<bool> DeleteNoteAsync(Note note);
    }
}
