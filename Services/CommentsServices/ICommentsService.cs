using OnlineNotes.Models;

namespace OnlineNotes.Services.CommentsServices
{
    public interface ICommentsService
    {
        Task<bool> CreateComment(Comment comment);
        Comment? GetCommentById(int? id);
        Task<bool> DeleteComment(Comment comment);
    }
}
