using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models.Quizzes;
using OnlineNotes.Models.Requests.Quiz;
using OnlineNotes.Services.NotesServices;

namespace OnlineNotes.Services.QuizzesServices
{
    public class QuizzesService : IQuizzesService
    {
        private readonly ReferencesRepository _refRep;
        private readonly ILogger<QuizzesService> _logger;
        private readonly INotesService _noteService;

        public QuizzesService(ReferencesRepository refRep, ILogger<QuizzesService> logger, INotesService notesService)
        {
            _refRep = refRep;
            _logger = logger;
            _noteService = notesService;
        }

        public async Task<bool> CreateQuizAsync(CreateQuizRequest quizRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Quiz>?> GetAllQuizzesToListAsync()
        {
            try
            {
                var quizzes = await _refRep.applicationDbContext.Quiz
                    .ToListAsync();
                return quizzes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetAllQuizzesToListAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        }

        public async Task<Quiz?> GetQuizByIdAsync(int? id)
        {
            try
            {
                if (id == null || id <= 0)
                {
                    return null;
                }

                var quiz = await _refRep.applicationDbContext.Quiz
                    .Include(q => q.Questions)
                        .ThenInclude(question => question.QuestionOptions)
                    .FirstOrDefaultAsync(quiz => quiz.Id == id);
                return quiz;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetQuizByIdAsync: {ErrorMessage}", ex.Message);
                return null;
            }
        } // TODO delete quiz + delete quiz request
    }
}
