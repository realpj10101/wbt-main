using api.Models.Helpers;

namespace api.Interfaces.Team;

public interface ITeamRepository
{
    public Task<TeamStatus> CreateAsync(ObjectId userId, string targetMemberUserName, CancellationToken cancellationToken);
}