using System.Text.RegularExpressions;

namespace OnlineNotes.ExtensionMethods
{
    public static class StringExtensions
    {

        public static int WordCount(this string contents)
        {
            string pattern = @"\b\w+\b";
            MatchCollection matches = Regex.Matches(contents, pattern);
            return matches.Count;
        }

        public static List<string> ParseQuestions(this string questions)
        {
            List<string> parsedQuestions = new List<string>();
            string[] splitQuestions = questions.Split("\n");
            foreach (string question in splitQuestions)
            {
                if (question.Length > 0)
                {
                    parsedQuestions.Add(question);
                }
            }
            return parsedQuestions;
        }
    }
}
