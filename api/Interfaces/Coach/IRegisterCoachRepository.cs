using api.DTOs.Coach_DTOs;

namespace api.Interfaces.Coach;

public interface IRegisterCoachRepository
{
    public Task<LoggedInCoachDto> RegisterCoachAsync(RegisterCoachDto coachInput, CancellationToken cancellationToken);
}