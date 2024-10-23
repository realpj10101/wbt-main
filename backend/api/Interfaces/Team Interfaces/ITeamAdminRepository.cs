using api.DTOs.Team;

namespace api.Interfaces;

public interface ITeamAdminRepository
{
    public Task<IEnumerable<PlayerWithRoleDto>> GetPlayersWithRolesAsync();
    public Task<bool> RemovePlayerAsync(string userName);
    public Task<bool> SuspendPlayerAsync(string userName);
}