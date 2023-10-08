using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Data;
using OnlineNotes.Models;

namespace OnlineNotes.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private int _noteId;
        //private Note? associatedNote;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Create(int noteId)
        {
            _noteId = noteId;
            ViewBag.Message = noteId;
            //associatedNote = _context?.Note?.FirstOrDefault(n => n.Id == _noteId); // TODO what if null
            //ViewBag.AssociatedNote = associatedNote;
            ViewBag.CreationDate = DateTime.Now;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Contents,CreationDate,NoteId")] Comment comment)
        {
            ModelState.Remove("Note"); // navigation property will be set later by EF based on 'NoteId'
            if (ModelState.IsValid)
            {
                // Save the comment to the database
                _context.Add(comment);
                await _context.SaveChangesAsync();

                // Redirect to the appropriate page (e.g., the note's details page)
                return RedirectToAction("Details", "Notes", new { id = comment.NoteId });
            } 

            return View(comment); // Show the form with validation errors
        }
    }
}
