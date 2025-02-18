using api.DTOs.Team_DTOs;
using api.Helpers;
using api.Models.Helpers;

namespace api.Interfaces.Teams;

public interface ITeamRepository
{
    public Task<ShowTeamDto?> CreateAsync(ObjectId userId, CreateTeamDto userInput, CancellationToken cancellationToken);
    public Task<TeamStatus> UpdateTeamAsync(ObjectId userId, UpdateTeamDto userInput, string targetTeamName, CancellationToken cancellationToken);
    public Task<PagedList<Team>?> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken);
}