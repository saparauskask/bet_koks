using System.Text;
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
            string[] splitByQuestions = questions.Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string questionText in splitByQuestions)
            {
                string[] questionLines = questionText.Split('\n');

                StringBuilder parsedQuestion = new StringBuilder();
                foreach (string line in questionLines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        parsedQuestion.AppendLine(line.Trim());
                    }
                }

                parsedQuestions.Add(parsedQuestion.ToString().Trim());
            }

            return parsedQuestions;
        }

        public static List<(string AnswerText, bool IsCorrect)> ParseAnswers(this string answers)
        {
            List<(string AnswerText, bool IsCorrect)> parsedAnswers = new();
            // Split each answer set by newline ("\n")
            string[] answerLines = answers.Split('\n'); // Index check?
            string trimmedLine = answerLines[1].Trim();
            string trimmedAnswer = answerLines[2].Trim();
            string correctAnswer = "";

            if (!string.IsNullOrWhiteSpace(trimmedAnswer))
            {
                string pattern = @"Correct Answer for question Q\d+ is: ([a-zA-Z.]+)";
                Match match = Regex.Match(trimmedAnswer, pattern);
                if (match.Success)
                {
                    correctAnswer = match.Groups[1].Value.Trim();
                }
            }

            // Check if the line is not empty
            if (!string.IsNullOrWhiteSpace(trimmedLine))
            {
                MatchCollection matches = Regex.Matches(trimmedLine, @"([a-zA-Z]\.\s[^.]+\.)");

                foreach (Match match in matches)
                {
                    bool isCorrect = false;
                    // Extract the matched group
                    string matchedGroup = match.Groups[1].Value;
                    // Extract the letter (a., b., c., etc.) and the answer text
                    string letter = matchedGroup.Substring(0, 2).Trim();
                    if (letter.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                    {
                        isCorrect = true;
                    }

                    string answerText = matchedGroup.Substring(2).Trim();

                    // Add each answer option to the list with IsCorrect set to false
                    parsedAnswers.Add(($"{letter}. {answerText}", isCorrect));
                }
            }

            return parsedAnswers;
        }

        public static string ExtractQuestions(this string contents)
        {
            string[] extractedLines = contents.Split('\n');
            string trimmedLine = extractedLines[0].Trim();
            if (!string.IsNullOrWhiteSpace(trimmedLine))
            {
                return trimmedLine;
            }
            return "";
        }
    }
}
