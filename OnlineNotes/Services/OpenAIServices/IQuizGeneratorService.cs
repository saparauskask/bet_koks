using OnlineNotes.Models.Enums;

namespace OnlineNotes.Services.OpenAIServices
{
    public interface IQuizGeneratorService
    {
        Task<string> GenerateQuiz(string noteContents, QuizDifficulty difficulty, int questionsCount);
    }
}
