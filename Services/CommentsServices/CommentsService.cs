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

        public async Task<bool> CreateComment (Comment comment)
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

        public async Task<bool> DeleteComment(Comment comment)
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

        public Comment? GetCommentById(int? id)
        {
            try
            {
                var comment = _context.Comment?.Find(id);
                if (comment != null)
                {
                    return comment;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
