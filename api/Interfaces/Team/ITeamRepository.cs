using api.DTOs.Team_DTOs;

namespace api.Interfaces.Team;

public interface ITeamRepository
{
    public Task<ShowTeamDto?> CreateAsync(ObjectId userId, CreateTeamDto userInput, CancellationToken cancellationToken);
    public Task<bool> UpdateTeamAsync(UpdateTeamDto userInput, string targetTeamName, CancellationToken cancellationToken);
}