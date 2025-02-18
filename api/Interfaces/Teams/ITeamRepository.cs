using api.DTOs.Team_DTOs;
using api.Helpers;

namespace api.Interfaces.Teams;

public interface ITeamRepository
{
    public Task<ShowTeamDto?> CreateAsync(ObjectId userId, CreateTeamDto userInput, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdateTeamAsync(UpdateTeamDto userInput, string targetTeamName, CancellationToken cancellationToken);
    public Task<PagedList<Team>?> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
}