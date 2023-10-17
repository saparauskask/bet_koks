using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;

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

        public async Task<bool> CreateCommentAsync (Comment comment)
        {
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

        public async Task<bool> DeleteCommentAsync(Comment comment)
        {
            try
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
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
    }
}
