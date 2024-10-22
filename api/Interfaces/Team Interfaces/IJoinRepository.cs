using api.Helpers;
using api.Models.Helpers;

namespace api.Interfaces.Team;

public interface IJoinRepository
{
    public Task<JoinStatus> CreateJoinAsync(ObjectId playerId, string targetTeamUserName, CancellationToken cancellationToken);
    public Task<JoinStatus> RemoveAsync(ObjectId playerId, string targetTeamUserName, CancellationToken cancellationToken);
    public Task<bool> CheckIsJoiningAsync(ObjectId playerId, RootModel rootModel, CancellationToken cancellationToken);
    // public Task<PagedList<RootModel>> GetAllAsync(JoinParams joinParams, CancellationToken cancellationToken);
}