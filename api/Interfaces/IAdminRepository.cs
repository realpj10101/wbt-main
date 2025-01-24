namespace api.Repositories.Player;

public interface IAdminRepository
{
    public Task<IEnumerable<PlayerWithRoleDto>> GetUsersWithRoleAsync();
    public Task<DeleteResult?> DeleteUserAsync(string targetMemberUserName, CancellationToken cancellationToken);
}