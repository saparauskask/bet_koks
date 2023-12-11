using Microsoft.EntityFrameworkCore;
using OnlineNotes.Data;
using OnlineNotes.Models;
using OnlineNotes.Models.Requests.Comments;

namespace OnlineNotes.Services.CommentsServices
{
    public class CommentsService : ICommentsService
    {
        private readonly ReferencesRepository _referencesRepository;
        private readonly ILogger<CommentsService> _logger;

        public CommentsService(ReferencesRepository referencesRepository, ILogger<CommentsService> logger)
        {
            _referencesRepository = referencesRepository;
            _logger = logger;
        }

        public async Task<bool> CreateCommentAsync(CreateCommentRequest commentReqest)
        {
            Comment comment = new Comment
            {
                Contents = commentReqest.Contents,
                NoteId = commentReqest.NoteId,
                CreationDate = DateTime.Now
            };

            try
            {
                _referencesRepository.applicationDbContext.Comment.Add(comment);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the comment: {ExceptionMessage}.", ex.Message);
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
                _referencesRepository.applicationDbContext.Comment.Remove(comment);
                await _referencesRepository.applicationDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the comment: {ExceptionMessage}", ex.Message);
                return false;
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(int? id)
        {
            if (id == null || id <= 0)
            {
                return null;
            }
            try
            {
                var comment = await _referencesRepository.applicationDbContext.Comment
                    .Where(c => c.Id == id)
                    .FirstOrDefaultAsync();
                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while retrieving comment with ID {CommentId}.", id);
                return null;
            }
        }

        public async Task<int> GetNoteIdFromCommentId(int commentId)
        {
            Comment? comment = await GetCommentByIdAsync(commentId);

            if (comment != null)
            {
                return comment.NoteId;
            }

            return 0;
        }
    }
}
