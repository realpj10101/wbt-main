using api.Helpers;
using api.Models.Helpers;

namespace api.Interfaces;

public interface IFollowCoachRepository
{
    public Task<FollowStatus> CreateFollowCoAsync(ObjectId coachId, string targetMemberUserName, CancellationToken cancellationToken);   
    public Task<FollowStatus> RemoveAsync(ObjectId coachId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<bool> CheckIsFollowingAsync(ObjectId coacId, RootModel rootModel, CancellationToken cancellationToken);
    public Task<PagedList<RootModel>> GetAllAsync(FollowParams followParams, CancellationToken cancellationToken);
}