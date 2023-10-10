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

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Create(int noteId)
        {
            _noteId = noteId;
            ViewBag.Message = noteId;
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

        public IActionResult Delete(int id)
        {
            if (_context.Comment == null)
            {
                return NotFound();
            }

            var comment = _context.Comment.Find(id);

            if (comment == null)
            {
                return NotFound();
            }

            ViewBag.Id = id;
            ViewBag.Contents = comment.Contents;
            ViewBag.CreationDate = comment.CreationDate;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);

            if (comment != null)
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Notes");
            }

            return NotFound();

        }
    }
}
