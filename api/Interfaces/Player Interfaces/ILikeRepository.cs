using api.Models.Helpers;

namespace api.Interfaces;

public interface ILikeRepository
{
    public Task<LikeStatus> CreateLikeAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<LikeStatus> RemoveLikeAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken);
    public Task<bool> CheckIsLikingAsync(ObjectId coachId, RootModel rootModel, CancellationToken cancellationToken);
}