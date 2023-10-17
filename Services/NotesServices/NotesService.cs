using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;

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

        public async Task<bool> CreateNoteAsync(Note note)
        {
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

        public async Task<bool> DeleteNoteAsync(Note note)
        {
            try
            {
                foreach (var comment in note.Comments.ToList())
                {
                    _context.Comment.Remove(comment);
                }
                
                _context.Note.Remove(note);
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

        public async Task<bool> UpdateNoteAsync(int id, Note note)
        {
            if (id != note.Id)
            {
                return false;
            }

            try
            {
                _context.Update(note);
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
