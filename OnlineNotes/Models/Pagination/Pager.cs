namespace OnlineNotes.Models.Pagination
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        public Pager() { }

        public Pager(int totalItems, int page)
        {
            int pageSize = (int)PaginationSettings.DefaultPageSize;
            int pageRangeStart = (int)Math.Floor((decimal)PaginationSettings.MaxVisiblePages / 2);
            int pageRangeEnd = (int)Math.Ceiling((decimal)PaginationSettings.MaxVisiblePages / 2);
            int totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            int currentPage = page;

            int startPage = 1;
            int endPage = 1;

            if (totalItems > 0)
            {
                startPage = Math.Clamp(currentPage - pageRangeStart, 1, totalPages);
                endPage = Math.Clamp(currentPage + pageRangeEnd, 0, totalPages);
            }
            

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
        }
    }
}
