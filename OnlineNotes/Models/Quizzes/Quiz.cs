using OnlineNotes.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineNotes.Models.Quizzes
{
    public class Quiz
    {
        public int Id { get; set; } // primary key
        public string UserId { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        [StringLength(60, ErrorMessage = "Title length must not exceed 60 characters")]
        public string Title { get; set; } = null!;
        public string NoteContents {  get; set; } = null!;
        public QuizDifficulty Difficulty { get; set; }
        [Range(1, 15, ErrorMessage = "Number of questions must be between 1 and 15")]
        public int QuestionsCount { get; set; }
        public ICollection<Question> Questions { get; } = new List<Question>();
        public int? Score { get; set; }
        public bool IsCompleted { get; set; }

        public Quiz()
        {

        }
        public Quiz(string userId, DateTime creationDate, string title, string noteContents, QuizDifficulty difficulty, int questionsCount)
        {
            UserId = userId;
            CreationDate = creationDate;
            Title = title;
            NoteContents = noteContents;
            Difficulty = difficulty;
            QuestionsCount = questionsCount;
        }
    }
}
