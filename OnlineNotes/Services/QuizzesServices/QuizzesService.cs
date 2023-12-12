using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.ExtensionMethods;
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
                var generatedQuiz = _quizGeneratorService.FakeGenerateQuiz(quizRequest.Title);
                // new code from this point
                if (!string.IsNullOrEmpty(generatedQuiz))
                {
                    quiz.NoteContents = generatedQuiz;
                    // FIXME add validation
                    await _referencesRepository.applicationDbContext.Quiz.AddAsync(quiz);
                    await _referencesRepository.applicationDbContext.SaveChangesAsync();
                    var result = await CreateQuestionsAsync(generatedQuiz, quiz.Id);
                    return quiz.Id;
                } else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateQuizAsync: {ErrorMessage}", ex.Message);
                return null;
            }
            
        }

        public async Task<bool> DeleteQuizAsync(int? id)
        {
            try
            {
                if (id == null)
                {
                    return false;
                }

                var quiz = await GetQuizByIdAsync(id);
                if (quiz == null)
                {
                    return false;
                }

                _referencesRepository.applicationDbContext.Remove(quiz);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in DeleteQuizAsync: {ErrorMessage}", ex.Message);
                return false;
            }
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
        }

        private async Task<bool> CreateQuestionsAsync(string generatedQuiz, int quizId)
        {
            List<string> parsedQuestions = generatedQuiz.ParseQuestions();
            foreach (var question in parsedQuestions)
            {
                var extractedQuestion = question.ExtractQuestions(); // check if not empty
                var parsedAnswers = question.ParseAnswers();
                var result = await CreateQuestionAsync(extractedQuestion, parsedAnswers, quizId);
                if (result == true)
                {
                    continue;
                }
                return false;
            }
            return false;
        }

        private async Task<bool> CreateQuestionAsync(string extractedQuestion, List<(string AnswerText, bool IsCorrect)> parsedAnswers, int quizId)
        {
            try
            {
                int correctAnswer = parsedAnswers.FindIndex(answer => answer.IsCorrect);
                var question = new Question
                {
                    QuizId = quizId,
                    QuestionText = extractedQuestion,
                    QuestionOptions = parsedAnswers.Select(answer => new QuestionOption { OptionText = answer.AnswerText }).ToList(),
                    CorrectAnswer = correctAnswer,
                    AnsweredCorrectly = null,
                    Explanation = null
                };
                _referencesRepository.applicationDbContext.Add(question);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();
                foreach (var option in question.QuestionOptions)
                {
                    option.QuestionId = question.Id;
                }

                await _referencesRepository.applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in CreateQuestionAsync: {ErrorMessage}", ex.Message);
                return false;
            }
        }
    }
}
