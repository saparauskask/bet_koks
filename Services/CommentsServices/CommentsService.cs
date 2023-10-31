using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Requests.Comments;

namespace OnlineNotes.Services.CommentsServices
{
    public class CommentsService : ICommentsService
    {
        private readonly ApplicationDbContext _context;

        public CommentsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Comment> GetCommentsFilteredByDateAsEnumerable(DateTime date)
        {
            List<Comment> comments = _context.Comment.ToList();
            comments = GenericFilterService<Comment>.FilterByCondition(comments,
                c => c.CreationDate == date);

            return comments.AsEnumerable();
        }

        public async Task<bool> CreateCommentAsync (CreateCommentRequest commentReqest)
        {
            Comment comment = new Comment();
            comment.Contents = commentReqest.Contents;
            comment.NoteId = commentReqest.NoteId;
            comment.CreationDate = DateTime.Now;

            try
            {
                _context.Comment.Add(comment);
                await _context.SaveChangesAsync();
                return true;
            } 
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> DeleteCommentAsync(DeleteCommentRequest commentRequest)
        {
            Comment? comment = await GetCommentByIdAsync(commentRequest.Id);

            if (comment == null)
            {
                return false;
            }

            try
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(int? id)
        {
            try
            {
                var comment = await _context.Comment
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();
                    return comment;
            }
            catch (Exception)
            {
                return null; // TODO do something with the exception
            }
        }

        public async Task<int> GetNoteIdFromCommentId(int commentId)
        {
            Comment? comment = await GetCommentByIdAsync(commentId);

            if (comment != null)
            {
                return comment.NoteId;
            }
            else
            {
                throw new Exception("Comment not found");
            }
        }
    }
}
