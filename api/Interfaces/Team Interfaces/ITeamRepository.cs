using api.DTOs.Team;
using api.Helpers;

namespace api.Interfaces.Team; 

public interface ITeamRepository
{
    public Task<PagedList<RootModel>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);

    public Task<TeamDto?> GetByIdAsync(string teamId, CancellationToken cancellationToken);

    public Task<TeamDto?> GetByUserNameAsync(string teamUserName, CancellationToken cancellationToken);
}