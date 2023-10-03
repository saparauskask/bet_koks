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

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment, int noteId)
        {
            if (ModelState.IsValid)
            {
                // Associate the comment with a specific note
                comment.NoteId = noteId;

                // Set the CreationDate property to the current date and time
                comment.CreationDate = DateTime.Now;

                // Save the comment to the database
                _context.Add(comment);
                await _context.SaveChangesAsync();

                // Redirect to the appropriate page (e.g., the note's details page)
                return RedirectToAction("Details", "Notes", new { id = noteId });
            }

            return View(comment); // Show the form with validation errors
        }
    }
}
