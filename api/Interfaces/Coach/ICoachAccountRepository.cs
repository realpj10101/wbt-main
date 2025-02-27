using api.DTOs.Coach_DTOs;

namespace api.Interfaces.Coach;

public interface ICoachAccountRepository
{
    public Task<LoggedInDto> RegisterCoachAsync(AccountDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInCoachDto> LoginAsync(LoginCoachDto coachInput, CancellationToken cancellationToken);
    public Task<LoggedInCoachDto?> ReloadLoggedInCoachAsync(string hashedUserId, string token, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdateLastActive(string loggedInCoachId, CancellationToken cancellationToken);
}