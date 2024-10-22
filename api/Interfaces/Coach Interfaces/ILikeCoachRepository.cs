using api.Models.Helpers;

namespace api.Interfaces;

public interface ILikeCoachRepository
{
    public Task<LikeStatus> CreateLikeCoAsync(ObjectId coachId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<LikeStatus> RemoveLikeCoAsync(ObjectId coacId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<bool> CheckIsLikingCoAsync(ObjectId coachId, RootModel rootModel, CancellationToken cancellationToken);
}