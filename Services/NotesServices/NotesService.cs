﻿using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;

namespace OnlineNotes.Services.NotesServices
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<NotesService> _logger;

        public NotesService(ApplicationDbContext context, IHttpContextAccessor contextAccessor, ILogger<NotesService> logger)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

        public NoteStatus? GetFilterStatus()
        {
            if (_contextAccessor.HttpContext != null)
            {
                string? filterStatusString = _contextAccessor.HttpContext.Session.GetString("FilterStatus");

                switch (filterStatusString)
                {
                    case "Public":
                        return NoteStatus.Public;
                    case "Draft":
                        return NoteStatus.Draft;
                    case "Archived":
                        return NoteStatus.Archived;
                    default:
                        return null;
                }
            }
            _logger.LogError("HttpContext is null.");
            return null;
        }

        public async Task<bool> CreateNoteAsync(CreateNoteRequest noteRequest)
        {
            Note note = new(noteRequest.Title, noteRequest.Contents, noteRequest.Status);

            try
            {
                _context.Note.Add(note);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Note with ID: {NoteId} was created successfully.", note.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the note: {ExceptionMessage}.", ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteNoteAsync(DeleteNoteRequest note)
        {
            Note? actualNote = await GetNoteAsync(note.Id);

            if (actualNote == null)
            {
                _logger.LogWarning("Note with ID: {noteId} was not found for deletion.", note.Id);
                return false;
            }

            var noteId = note.Id;
            try
            {
                foreach (var comment in actualNote.Comments.ToList())
                {
                    _context.Comment.Remove(comment);
                }
                
                _context.Note.Remove(actualNote);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Note with ID: {noteId} was deleted successfully.", noteId);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the note: {ExceptionMessage}", ex.Message);
                return false;
            }
        }

        public async Task<Note?> GetNoteAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("GetNoteAsync: Requested Note with ID: null.");
                return null;
            }

            var note = await _context.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .FirstOrDefaultAsync(m => m.Id == id);

            if (note != null)
            {
                _logger.LogInformation("Retrieved Note with ID: {NoteId}.", note.Id);
            }
            else
            {
                _logger.LogWarning("Note with ID: {NoteId} was not found.", id);
            }

            return note;
        }

        public async Task<IEnumerable<Note>?> GetFilteredNotesToListAsync(NoteStatus? filterStatus)
        {
            try
            {
                if (_contextAccessor.HttpContext != null)
                {
                        _contextAccessor.HttpContext.Session.SetString("FilterStatus", filterStatus.ToString());
                }

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
                _logger.LogError(ex, "An error occurred in GetFilteredNotesToListAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<Note>?> GetIndexedNotesToListAsync(string term)
        {
            try
            {
                // Makes search term and Note title lowercase to make searching case insensitive
                string lowerTerm = term.ToLower();
                var notes = await _context.Note.Where(note => note.Title.ToLower().Contains(lowerTerm)).ToListAsync();
                return notes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetIndexedNotesToListAsync: {ErrorMessage}", ex.Message);
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
                _logger.LogInformation("Note with ID: {NoteId} updated successfully.", note.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Note with ID: {NoteId}", note.Id);
                return false;
            }
        }
    }
}
