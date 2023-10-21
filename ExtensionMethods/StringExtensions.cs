namespace OnlineNotes.ExtensionMethods
{
    public static class StringExtensions
    {

        public static int WordCount(this string contents)
        {
            char[] delimiters = { ' ', '.', '?' };
            string[] words = contents.Split(delimiters, StringSplitOptions.RemoveEmptyEntries); // splits string into substrings based on the delimeters

            return words.Length;
        }
    }
}
