namespace OnlineNotes.Data
{
    public class ReferencesRepository
    {
        public readonly ApplicationDbContext applicationDbContext;
        public readonly IHttpContextAccessor httpContextAccessor;

        public ReferencesRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            this.applicationDbContext = applicationDbContext;
            this.httpContextAccessor = httpContextAccessor;
        }
    }
}
