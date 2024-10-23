namespace api.Interfaces;

public interface ICoachAdminRepository
{
    public Task<IEnumerable<CoachWithRoleDto>> GetCoachesWithRolesAsync();
    public Task<bool> DeleteCoachAsync(string userName);
    public Task<bool> SuspendCoachAsync(string userName);
}
