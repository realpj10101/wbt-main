using api.Helpers;
using api.Models.Helpers;

namespace api.Interfaces.Player;

public interface ILikeRepository
{
    public Task<LikeStatus> CreateAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<LikeStatus> DeleteAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<bool> CheckIsLikingAsync(ObjectId playerId, AppUser appUser, CancellationToken cancellationToken);
    public Task<PagedList<AppUser>> GetAllAsync(LikeParams likeParams, CancellationToken cancellationToken);
}