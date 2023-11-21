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
    }
}
