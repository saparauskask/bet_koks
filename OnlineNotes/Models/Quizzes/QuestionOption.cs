namespace OnlineNotes.Models.Quizzes
{
    public class QuestionOption
    {
        public int Id { get; set; }
        public int QuestionId { get; set; } // foreign key to Question
        public Question Question { get; set; } = null!; // navigation to principal
        public string OptionText { get; set; } = null!;
    }
}
