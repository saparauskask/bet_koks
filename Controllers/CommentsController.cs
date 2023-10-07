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
        private Note? associatedNote;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Create(int noteId)
        {
            _noteId = noteId;
            ViewBag.Message = noteId;
            associatedNote = _context?.Note?.FirstOrDefault(n => n.Id == _noteId); // TODO what if null
                if (associatedNote == null) { return NotFound(); }
            ViewBag.AssociatedNote = associatedNote;
            ViewBag.CreationDate = DateTime.Now;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Contents,CreationDate,NoteId,Note")] Comment comment)
        {
            Console.WriteLine("IMPORTANT");
            Console.WriteLine("Id of comment: " + comment.Id);
            Console.WriteLine("NoteID from comment: " + comment.NoteId);
            Console.WriteLine("NoteID from Note: " + comment.Note.Id);
            Console.WriteLine("CreationDate: " + comment.CreationDate);
            Console.WriteLine("!IMPORTANT");

            if (ModelState.IsValid)
            {
                // Save the comment to the database
                _context.Add(comment); // TODO: verify that not null
                await _context.SaveChangesAsync();

                // Redirect to the appropriate page (e.g., the note's details page)
                return RedirectToAction("Details", "Notes", new { id = comment.NoteId });
            } else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            return View(comment); // Show the form with validation errors
        }
    }
}
