namespace OnlineNotes.Services
{
    public class GenericFilterService<T>
    {
        public static List<T> FilterByCondition(IEnumerable<T> items, Func<T, bool> condition) =>
            items.Where(condition).ToList();
    }
}
