namespace OnlineNotes.Models.Quizzes
{
    public class Question
    {
        public int Id { get; set; } // primary key
        public int QuizId{ get; set; } // foreign key to Quiz
        public Quiz Quiz { get; set; } = null!; // navigation to principal
        public string QuestionText { get; set; } = null!;
        public List<QuestionOption> QuestionOptions { get; set; } = null!;
        public int? CorrectAnswer { get; set; } // index of the correct answer in QuestionOptions
        public bool? AnsweredCorrectly { get; set; }
        public string? Explanation { get; set; } // only if AnsweredCorrectly is false

    }
}
