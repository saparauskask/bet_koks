using OnlineNotes.Models.Quizzes;
using OnlineNotes.Models.Requests.Quiz;

namespace OnlineNotes.Services.QuizzesServices
{
    public interface IQuizzesService
    {
        Task<Quiz?> GetQuizByIdAsync(int? id);
        Task<IEnumerable<Quiz>?> GetAllQuizzesToListAsync();
        Task<int?> CreateQuizAsync(CreateQuizRequest quizRequest);
        Task<bool> DeleteQuizAsync(int? id);
    }
}
