using api.Helpers;
using api.Models.Helpers;

namespace api.Interfaces.Player;

public interface IFollowRepository
{
    public Task<FollowStatus> CreateAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<FollowStatus> DeleteAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<bool> CheckIsFollowingAsync(ObjectId playerId, AppUser appUser, CancellationToken cancellationToken);
    public Task<PagedList<AppUser>> GetAllAsync(FollowParams followParams, CancellationToken cancellationToken);    
}