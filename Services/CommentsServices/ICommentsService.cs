using OnlineNotes.Models;
using OnlineNotes.Models.Requests.Comments;

namespace OnlineNotes.Services.CommentsServices
{
    public interface ICommentsService
    {
        IEnumerable<Comment>? GetCommentsFilteredByDateAsEnumerable(DateTime date);
        Task<bool> CreateCommentAsync(CreateCommentRequest commentReqest);
        Task<Comment?> GetCommentByIdAsync(int? id);
        Task<bool> DeleteCommentAsync(DeleteCommentRequest commentRequest);
        Task<int> GetNoteIdFromCommentId(int commentId);
    }
}
