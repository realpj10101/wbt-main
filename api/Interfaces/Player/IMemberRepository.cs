using api.Helpers;

namespace api.Interfaces.Player;

public interface IMemberRepository
{
    public Task<PagedList<AppUser>?> GetAllAsync(MemberParams memberParams, CancellationToken cancellationToken);
    public Task<PlayerDto?> GetByIdAsync(string playerId, CancellationToken cancellationToken);
    public Task<PlayerDto?> GetByUserNameAsync(string playerUserName, string userIdHashed, CancellationToken cancellationToken);
}