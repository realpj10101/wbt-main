using api.Helpers;

namespace api.Interfaces;
public interface ICoachRepository
{
    public Task<PagedList<RootModel>> GetAllCoachsAsync(PaginationParams paginationParams, CancellationToken cancellationToken);

    public Task<CoachDto?> GetByCoachIdAsync(string coachId, CancellationToken cancellationToken);

    public Task<CoachDto?> GetByCoachUserNameAsync(string coachUserName, CancellationToken cancellationToken);
}
