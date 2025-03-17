using api.DTOs.Coach_DTOs;

namespace api.Interfaces.Coach;

public interface ICoachAccountRepository
{
    public Task<LoggedInDto> RegisterCoachAsync(RegisterDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto> LoginAsync(LoginDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto?> ReloadLoggedInCoachAsync(string hashedUserId, string token, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdateLastActive(string loggedInCoachId, CancellationToken cancellationToken);
}