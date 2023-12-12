using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models;
using OnlineNotes.Models.Requests.Comments;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Contents,CreationDate,NoteId")] CreateCommentRequest comment)
        {
            ModelState.Remove("Note"); // navigation property will be set later by EF based on 'NoteId'

            if (ModelState.IsValid)
            {
                var result = await _commentsService.CreateCommentAsync(comment);

                if (result)
                {
                    return RedirectToAction("Details", "Notes", new { id = comment.NoteId });
                }

            }
            return View(comment); // Show the form with validation errors
        }

        public async Task<IActionResult> DeleteAsync(int id)
        {
            var comment = await _commentsService.GetCommentByIdAsync(id);

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
        public async Task<IActionResult> DeleteConfirmed(DeleteCommentRequest comment)
        {
            int noteId = await _commentsService.GetNoteIdFromCommentId(comment.Id);
            var result = await _commentsService.DeleteCommentAsync(comment);

            if (result && noteId != 0)
            {
                return RedirectToAction("Details", "Notes", new { id = noteId });
            }
            return NotFound();
        }
    }
}
