using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.ExtensionMethods;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;
using OnlineNotes.Services.NotesServices;
using OnlineNotes.Services.OpenAIServices;
using OnlineNotes.Services.RatingServices;
using Microsoft.AspNetCore.Identity;
using OnlineNotes.Models.Requests.NoteRating;
using OnlineNotes.Exceptions;
using OnlineNotes.Data.Migrations;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class NotesController : Controller
    {
        private readonly IOpenAIService _openAIService;
        private readonly INotesService _notesService;
        private readonly INoteRatingService _ratingService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<NotesService> _logger;

        public NotesController(IOpenAIService openAIService, INotesService notesService, INoteRatingService ratingService, UserManager<IdentityUser> userManager, ILogger<NotesService> logger)
        {
            _openAIService = openAIService;
            _notesService = notesService;
            _ratingService = ratingService;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Notes
        public async Task<IActionResult> Index(int page = 1, string? errorMessage = null)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            // Filtering:
            var notes = await _notesService.GetFilteredNotesToListAsync(_notesService.GetFilterStatus(), userId);

            if (notes == null)
            {
                return Error();
            }

            // Sorting:
            notes = _notesService.GetSortedNotes(notes);

            // Pagination:
            var data = _notesService.GetPagedNotes(notes, page, this);

            ViewBag.ErrorMessage = errorMessage;
            return View(data);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id) // TODO exception
        {
            try
            {
                IdentityUser user = await _userManager.GetUserAsync(User);
                var userId = user.Id;

                var note = await _notesService.GetNoteAsync(id);

                if (note == null)
                {
                    return NotFound();
                }
                var wordCount = note.Contents.WordCount();
                ViewBag.Count = wordCount;
                ViewBag.NoteId = id;
                if (note.Status == NoteStatus.Draft && userId != note.UserId)
                {
                    throw new NoteAccessDeniedException(userId, note.Id, "read");
                }
                return View(note);
            }
            catch (NoteAccessDeniedException ex)
            {
                _logger.LogError($"Access to the note (Id: {ex.NoteId}) during {ex.Operation} operation was denied");
                return RedirectToAction(nameof(Index), new {errorMessage = ex.GetErrorMessage()});
            }
        }

        // GET: Notes/Create
        public async Task<IActionResult> Create()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            ViewBag.UserId = userId;

            return View();
        }

        public IActionResult Filter(NoteStatus? status)
        {
            _notesService.SetFilterStatus(status);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string term)
        {
            if (!String.IsNullOrWhiteSpace(term))
            {
                var notes = await _notesService.GetIndexedNotesToListAsync(term);
                return View("Index", notes);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Sort(int sortOrder)
        {
            _notesService.SetSortStatus(sortOrder);
            return RedirectToAction(nameof(Index));
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Contents,Status, UserId")] CreateNoteRequest note)
        {
            if (ModelState.IsValid)
            {
                var result = await _notesService.CreateNoteAsync(note);

                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                return Error();
            }
            return View(note);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                IdentityUser user = await _userManager.GetUserAsync(User);
                var userId = user.Id;
                var note = await _notesService.GetNoteAsync(id);
                if (note == null)
                {
                    return NotFound();
                }
                if (note.Status == NoteStatus.Draft && userId != note.UserId)
                {
                    throw new NoteAccessDeniedException(userId, note.Id, "edit");
                }

                return View(note);
            }
            catch (NoteAccessDeniedException ex)
            {
                _logger.LogError($"Access to the note (Id: {ex.NoteId}) during {ex.Operation} operation was denied");
                return RedirectToAction(nameof(Index), new { errorMessage = ex.GetErrorMessage() });
            }
            
        }

        public async Task<IActionResult> ExplainTask(string input)
        {
            string completionResult = await _openAIService.CompleteSentence(input);
            return Content(completionResult, "text/plain");
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Title,Contents,Status, AvgRating, UserId")] EditNoteRequest note)
        {
            if (ModelState.IsValid)
            {
                var result = await _notesService.UpdateNoteAsync(note);

                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                IdentityUser user = await _userManager.GetUserAsync(User);
                var userId = user.Id;
                var note = await _notesService.GetNoteAsync(id);

                if (note == null)
                {
                    return NotFound();
                }

                if (note.Status == NoteStatus.Draft && userId != note.UserId)
                {
                    throw new NoteAccessDeniedException(userId, note.Id, "delete");
                }
                return View(note);
            }
            catch(NoteAccessDeniedException ex)
            {
                _logger.LogError($"Access to the note (Id: {ex.NoteId}) during {ex.Operation} operation was denied");
                return RedirectToAction(nameof(Index), new { errorMessage = ex.GetErrorMessage() });
            }
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DeleteNoteRequest note)
        {
            if (note == null)
            {
                return NotFound();
            }

            var result = await _notesService.DeleteNoteAsync(note);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRating(int rating, int noteId)
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            var note = await _notesService.GetNoteAsync(noteId);

            if (note != null)
            {
                var noteRatingId = _notesService.GetNoteRatingIdByUserId(note, user.Id);
                bool result;

                if(noteRatingId != null)
                {
                    var ratingRequest = new EditNoteRatingRequest
                    {
                        Id = noteRatingId ?? 0,
                        RatingValue = rating,
                        CreationDate = DateTime.Now
                    };

                    result = await _ratingService.UpdateNoteRatingAsync(ratingRequest);
                } else
                {
                    var ratingRequest = new CreateNoteRatingRequest
                    {
                        UserId = user.Id,
                        RatingValue = rating,
                        CreationDate = DateTime.Now,
                        Note = note
                    };

                    result = await _ratingService.CreateNoteRatingAsync(ratingRequest);
                }

                if (result)
                {
                    var updatedNoteResult = await _notesService.CalculateAvgRating(note);

                    if (updatedNoteResult)
                    {
                        return RedirectToAction("Details", new { id = note.Id });
                    }
                }
            }
            return Error();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
