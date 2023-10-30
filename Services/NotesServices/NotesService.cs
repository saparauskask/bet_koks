using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;

namespace OnlineNotes.Services.NotesServices
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext _context;
        public NotesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Note> GetNotesAsEnumerable()
        {
            List<Note> notes = _context.Note.ToList();
            return notes.AsEnumerable();
        }

        public async Task<bool> CreateNoteAsync(CreateNoteRequest noteRequest)
        {
            Note note = new(noteRequest.Title, noteRequest.Contents, noteRequest.Status);

            try
            {
                _context.Note.Add(note);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteNoteAsync(DeleteNoteRequest note)
        {
            Note? actualNote = await GetNoteAsync(note.Id);

            try
            {
                foreach (var comment in actualNote.Comments.ToList())
                {
                    _context.Comment.Remove(comment);
                }
                
                _context.Note.Remove(actualNote);
                await _context.SaveChangesAsync();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public async Task<Note?> GetNoteAsync(int? id)
        {
            if (id == null)
            {
                return null;
            }

            var note = await _context.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .FirstOrDefaultAsync(m => m.Id == id);

            return note;
        }

        public async Task<List<Note>?> GetNotesToListAsync()
        {
            try
            {
                var notes = await _context.Note.ToListAsync();
                return notes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "An eror occured");
                return null;
            }
        }

        public async Task<IEnumerable<Note>?> GetFilteredNotesToListAsync(NoteStatus? filterStatus)
        {
            try
            {
                if (filterStatus.HasValue)
                {
                    var notes = await _context.Note.Where(note => note.Status == filterStatus).ToListAsync();
                    return notes.AsEnumerable();
                }
                else
                {
                    var notes = await _context.Note.ToListAsync();
                    return notes.AsEnumerable();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "An eror occured");
                return null;
            }
        }

        public async Task<bool> UpdateNoteAsync(EditNoteRequest note)
        {
            Note actualNote = new(note.Title, note.Contents, note.Status) { Id = note.Id };

            try
            {
                _context.Update(actualNote);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
