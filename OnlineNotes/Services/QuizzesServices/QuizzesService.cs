using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models.Quizzes;
using OnlineNotes.Models.Requests.Quiz;
using OnlineNotes.Services.OpenAIServices;

namespace OnlineNotes.Services.QuizzesServices
{
    public class QuizzesService : IQuizzesService
    {
        private readonly ReferencesRepository _referencesRepository;
        private readonly ILogger<QuizzesService> _logger;
        private readonly IQuizGeneratorService _quizGeneratorService;

        public QuizzesService(ReferencesRepository referencesRepository, ILogger<QuizzesService> logger, IQuizGeneratorService quizGeneratorService)
        {
            _referencesRepository = referencesRepository;
            _logger = logger;
            _quizGeneratorService = quizGeneratorService;
        }

        public async Task<int?> CreateQuizAsync(CreateQuizRequest quizRequest)
        {
            try
            {
                var quiz = new Quiz(quizRequest.UserId, quizRequest.CreationDate, quizRequest.Title, quizRequest.NoteContents, quizRequest.Difficulty, quizRequest.QuestionsCount);
                var generatedQuiz = await _quizGeneratorService.GenerateQuiz(quiz.NoteContents, quiz.Difficulty, quiz.QuestionsCount); // IT WILL BE FIXED
                quiz.NoteContents = generatedQuiz;
                await _referencesRepository.applicationDbContext.Quiz.AddAsync(quiz);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();
                return quiz.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateQuizAsync: {ErrorMessage}", ex.Message);
                return null;
            }
            // logic for generating questions, options based on the configuation
        }

        public async Task<IEnumerable<Quiz>?> GetAllQuizzesToListAsync()
        {
            try
            {
                var quizzes = await _referencesRepository.applicationDbContext.Quiz
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

                var quiz = await _referencesRepository.applicationDbContext.Quiz
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
