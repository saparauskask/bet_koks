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

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class NotesController : Controller
    {
        private readonly IOpenAIService _openAIService;
        private readonly INotesService _notesService;
        private readonly INoteRatingService _ratingService;
        private readonly UserManager<IdentityUser> _userManager;

        public NotesController(IOpenAIService openAIService, INotesService notesService, INoteRatingService ratingService, UserManager<IdentityUser> userManager)
        {
            _openAIService = openAIService;
            _notesService = notesService;
            _ratingService = ratingService;
            _userManager = userManager;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            // Filtering:
            var notes = await _notesService.GetFilteredNotesToListAsync(_notesService.GetFilterStatus());

            if (notes == null)
            {
                return Error();
            }

            // Sorting: 
            return View(_notesService.GetSortedNotes(notes));
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var note = await _notesService.GetNoteAsync(id);

            if (note == null)
            {
                return NotFound();
            }
            var wordCount = note.Contents.WordCount();
            ViewBag.Count = wordCount;
            ViewBag.NoteId = id;
            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
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
        public async Task<IActionResult> Create([Bind("Id,Title,Contents,Status")] CreateNoteRequest note)
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
            var note = await _notesService.GetNoteAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            return View(note);
        }

        public async Task<IActionResult> ExplainTask(string input)
        {
            string completionResult = await _openAIService.CompleteSentence(input);
            return Content(completionResult, "text/plain");
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Title,Contents,Status, AvgRating")] EditNoteRequest note)
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
            var note = await _notesService.GetNoteAsync(id);

            if (note == null)
            {
                return NotFound();
            }
            return View(note);
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
        public async Task<IActionResult> SubmitRating(int rating, int id)
        {// TODO check if note is not null!
            var note = await _notesService.GetNoteAsync(id);

            if (note != null)
            {
                var ratingRequest = new CreateNoteRatingRequest();
                IdentityUser user = await _userManager.GetUserAsync(User);

                ratingRequest.UserId = user.Id;
                ratingRequest.RatingValue = rating;
                ratingRequest.CreationDate = DateTime.Now;
                ratingRequest.Note = note;

                var result = await _ratingService.CreateNoteRatingAsync(ratingRequest);

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
