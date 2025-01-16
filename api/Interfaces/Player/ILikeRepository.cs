using api.Models.Helpers;

namespace api.Interfaces.Player;

public interface ILikeRepository
{
    public Task<LikeStatus> CreateAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<LikeStatus> DeleteAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
}