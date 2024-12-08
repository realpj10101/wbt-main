using api.Helpers;

namespace api.Interfaces.Player;

public interface IMemberRepository
{
    public Task<PagedList<AppUser>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
    
    public Task<PlayerDto?> GetByIdAsync(string playerId, CancellationToken cancellationToken);
    
    public Task<PlayerDto?> GetByUserNameAsync(string playerUserName, CancellationToken cancellationToken);
}