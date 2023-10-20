using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Data.Migrations;
using OnlineNotes.Models;
using OnlineNotes.Services.CommentsServices;

namespace OnlineNotes.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentsService _commentsService;
        private int _noteId;

        public CommentsController(ICommentsService commentsService) // Dependency Injection principle
        {
            _commentsService = commentsService;
        }


        public IActionResult Create(int noteId)
        {
            _noteId = noteId;
            ViewBag.Message = noteId;
            ViewBag.CreationDate = DateTime.Now;
            return View();
        }

        public IActionResult Filter(int noteId)
        {
            return RedirectToAction("Details", "Notes", new { id = noteId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Contents,CreationDate,NoteId")] Comment comment)
        {
            ModelState.Remove("Note"); // navigation property will be set later by EF based on 'NoteId'

            if (ModelState.IsValid)
            {
                var result = await _commentsService.CreateCommentAsync(comment);

                if (result)
                {
                    return RedirectToAction("Details", "Notes", new { id = comment.NoteId });
                }

                // TODO Redirect to the appropriate page (e.g., the note's details page)
            } 
            return View(comment); // Show the form with validation errors
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var comment = await _commentsService.GetCommentByIdAsync(id);

            if (comment  == null)
            {
                return NotFound();
            }

            ViewBag.Id = id;
            ViewBag.Contents = comment.Contents;
            ViewBag.CreationDate = comment.CreationDate;
            return View();
        }

        [HttpPost, ActionName("Filter")]
        public async Task<IActionResult> FilterByToday(int noteId)
        {
            return RedirectToAction("Details", "Notes", new { id = noteId });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _commentsService.GetCommentByIdAsync(id);

            if (comment != null)
            {
               var result = await _commentsService.DeleteCommentAsync(comment);

                if (result)
                {
                    return RedirectToAction("Index", "Notes");
                }
                return NotFound();
            }
            return NotFound();

        }
    }
}
