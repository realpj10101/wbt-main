using api.Helpers;

namespace api.Interfaces;
public interface IPlayerRepository
{
    public Task<PagedList<RootModel>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
    
    public Task<PlayerDto?> GetByIdAsync(string playerId, CancellationToken cancellationToken);

    public Task<PlayerDto?> GetByUserNameAsync(string playerUserName, CancellationToken cancellationToken);
}
