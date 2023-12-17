using OnlineNotes.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace OnlineNotes.Models.Requests.Quiz
{
    public class CreateQuizRequest
    {
        public string UserId { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        [StringLength(60, ErrorMessage = "Title length must not exceed 60 characters")]
        public string Title { get; set; } = null!;
        public string NoteContents { get; set; } = null!;
        public QuizDifficulty Difficulty { get; set; }
        [Range(2, 5, ErrorMessage = "Number of questions must be between 2 and 15")]
        public int QuestionsCount { get; set; }
        public bool IsCompleted = false;
    }
}
