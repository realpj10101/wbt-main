using api.Helpers;
using api.Models.Helpers;

namespace api.Interfaces;

public interface IFollowRepository
{
    public Task<FollowStatus> CreateFollowAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<FollowStatus> RemoveAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<bool> CheckIsFollowingAsync(ObjectId playerId, RootModel rootModel, CancellationToken cancellationToken);
    public Task<PagedList<RootModel>> GetAllAsync(FollowParams followParams, CancellationToken cancellationToken);
}