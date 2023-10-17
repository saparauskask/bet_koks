using OnlineNotes.Models;

namespace OnlineNotes.Services.CommentsServices
{
    public interface ICommentsService
    {
        IEnumerable<Comment> GetCommentsFilteredByDateAsEnumerable(DateTime date);
        Task<bool> CreateCommentAsync(Comment comment);
        Task<Comment?> GetCommentByIdAsync(int? id);
        Task<bool> DeleteCommentAsync(Comment comment);
    }
}
