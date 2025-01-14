using api.Models.Helpers;

namespace api.Repositories.Player;

public interface ICommentRepository
{
    public Task<CommentStatus> CreateAsync(ObjectId userId, string targetMemberUserName, string content, CancellationToken cancellationToken);
    // public Task<CommentStatus> DeleteAsync(ObjectId userId, string targetMemberUserName, CancellationToken cancellationToken);
}