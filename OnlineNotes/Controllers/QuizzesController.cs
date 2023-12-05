using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineNotes.Services.QuizzesServices;

namespace OnlineNotes.Controllers
{
    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<QuizzesController> _logger;
        private readonly IQuizzesService _quizzesService;

        public QuizzesController(UserManager<IdentityUser> userManager, ILogger<QuizzesController> logger, IQuizzesService quizzesService)
        {
            _userManager = userManager;
            _logger = logger;
            _quizzesService = quizzesService;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _quizzesService.GetAllQuizzesToListAsync();
            if (quizzes == null)
            {
                return NotFound();
            }
            return View(quizzes); // return quizz
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(CreateQuizRequest quiz) // lacking async method to be called
        //{
        //    if (ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //}
    }
}
