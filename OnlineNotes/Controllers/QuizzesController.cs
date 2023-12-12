using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Models.Quizzes;
using OnlineNotes.Models.Requests.Quiz;
using OnlineNotes.Services.NotesServices;
using OnlineNotes.Services.QuizzesServices;

namespace OnlineNotes.Controllers
{
    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<QuizzesController> _logger;
        private readonly IQuizzesService _quizzesService;
        private readonly INotesService _notesService;

        public QuizzesController(UserManager<IdentityUser> userManager, ILogger<QuizzesController> logger, IQuizzesService quizzesService, INotesService notesService)
        {
            _userManager = userManager;
            _logger = logger;
            _quizzesService = quizzesService;
            _notesService = notesService;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _quizzesService.GetAllQuizzesToListAsync();
            if (quizzes == null)
            {
                return NotFound();
            }
            return View(quizzes);
        }

        public async Task<IActionResult> Create()
        {
            IdentityUser user = await _userManager.GetUserAsync(User);
            var userId = user.Id;
            ViewBag.UserId = userId;
            var notes = await _notesService.GetAllNotesToListAsync();
            if (notes != null && notes.Any())
            {
                ViewBag.Notes = notes;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz) // lacking async method to be called
        {
            quiz.CreationDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                CreateQuizRequest quizRequest = new()
                {
                    UserId = quiz.UserId,
                    CreationDate = quiz.CreationDate,
                    Title = quiz.Title,
                    NoteContents = quiz.NoteContents,
                    Difficulty = quiz.Difficulty,
                    QuestionsCount = quiz.QuestionsCount
                };
                var quizeId = await _quizzesService.CreateQuizAsync(quizRequest);
                if (quizeId != null && quizeId > 0)
                {
                    return RedirectToAction("QuizAttempt", new { id = quizeId });
                }

                return RedirectToAction("Index");
            }
            return View(quiz);
        }

        public async Task<IActionResult> QuizAttempt(int id)
        {
            var quiz = await _quizzesService.GetQuizByIdAsync(id);
            if (quiz != null && id >= 0)
            {
                return View(quiz);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("Quizzes/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await _quizzesService.DeleteQuizAsync(id);
            if (result == true)
            {
                return RedirectToAction("Index");
            } else
            {
                return NotFound();
            }
        }
    }
}
