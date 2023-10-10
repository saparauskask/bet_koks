using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Services.OpenAIServices;

namespace OnlineNotes.Controllers
{
    [Authorize] // restricts access to a controller to only authenticated users
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IOpenAIService _openAIService;

        public NotesController(ApplicationDbContext context, IOpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
              return _context.Note != null ? 
                          View(await _context.Note.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Note'  is null.");
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Note == null)
            {
                return NotFound();
            }

            ViewBag.NoteId = id;
            var note = await _context.Note
                .Include(n => n.Comments) // Include the Comments navigation property
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Contents")] Note note)
        {
            if (ModelState.IsValid)
            {
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Note == null)
            {
                return NotFound();
            }

            var note = await _context.Note.FindAsync(id);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Contents")] Note note)
        {
            if (id != note.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(note);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Note == null)
            {
                return NotFound();
            }

            var note = await _context.Note
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Note == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Note'  is null.");
            }

            var note = await _context.Note
                .Include(n => n.Comments) // Include the Comments related to the Note
                .FirstOrDefaultAsync(n => n.Id == id);
            if (note != null)
            { 
                foreach (var comment in note.Comments.ToList())
                {
                    _context.Comment.Remove(comment);
                }

                _context.Note.Remove(note);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
          return (_context.Note?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
