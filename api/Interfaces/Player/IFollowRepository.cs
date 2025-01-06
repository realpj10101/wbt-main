using api.Models.Helpers;

namespace api.Interfaces.Player;

public interface IFollowRepository
{
    public Task<FollowStatus> CreateAsync(ObjectId playerId, string targetPlayerUserName, CancellationToken cancellationToken);
}