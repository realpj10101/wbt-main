using api.DTOs.Helpers;
using api.DTOs.Team_DTOs;

namespace api.Repositories.Player;

public interface IAdminRepository
{
    public Task<IEnumerable<PlayerWithRoleDto>> GetUsersWithRoleAsync();
    public Task<DeleteResult?> DeleteUserAsync(string targetMemberUserName, CancellationToken cancellationToken);
    public Task<OperationResult<ShowTeamDto>> UpdateVerifiedStatus(string teamName, CancellationToken cancellationToken);
    public Task<OperationResult<ShowTeamDto>> UpdateRejectStatus(string teamName, UpdateRejectStatus reason, CancellationToken cancellationToken);
}