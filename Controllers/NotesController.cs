using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.ExtensionMethods;
using OnlineNotes.Models;
using OnlineNotes.Models.Enums;
using OnlineNotes.Models.Requests.Note;
using OnlineNotes.Services.NotesServices;
using OnlineNotes.Services.OpenAIServices;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class NotesController : Controller
    {
        private readonly IOpenAIService _openAIService;
        private readonly INotesService _notesService;

        public NotesController(IOpenAIService openAIService, INotesService notesService)
        {
            _openAIService = openAIService;
            _notesService = notesService;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            var notes = await _notesService.GetFilteredNotesToListAsync(_notesService.GetFilterStatus());
              
            if (notes == null)
            {
                return Error();
            }

            return View(notes);
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

        public async Task<IActionResult> Filter(NoteStatus? status)
        {
            var notes = await _notesService.GetFilteredNotesToListAsync(status);
            if (notes == null)
            {
                return Error();
            }
            return View("Index", notes);
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
        public async Task<IActionResult> Edit([Bind("Id,Title,Contents,Status")] EditNoteRequest note)
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
