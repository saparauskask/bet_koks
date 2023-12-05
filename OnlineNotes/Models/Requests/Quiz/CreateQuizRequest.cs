using OnlineNotes.Models.Enums;

namespace OnlineNotes.Models.Requests.Quiz
{
    public class CreateQuizRequest
    {
        public string UserId { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string Title { get; set; } = null!; // should i add constraints here?
        public QuizDifficulty Difficulty { get; set; }
        //what about the collection of strings?
        public bool IsCompleted = false;
    }
}
