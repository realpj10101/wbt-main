namespace api.Interfaces;

public interface IAdminRepository
{
    public Task<IEnumerable<PlayerWithRoleDto>> GetPlayersWithRolesAsync();
    public Task<bool> DeletePlayerAsync(string userName);
    public Task<bool> SuspendPlayerAsync(string userName);
}   
