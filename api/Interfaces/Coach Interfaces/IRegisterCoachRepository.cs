namespace api.Interfaces;

public interface IRegisterCoachRepository
{
    public Task<LoggedInCoachDto> CreateCoachAsync(RegisterCoachDto coachInput, CancellationToken cancellationToken);

    public Task<LoggedInCoachDto> LoginCoachAsync(LoginCoachDto coachInput, CancellationToken cancellationToken);

    public Task<LoggedInCoachDto?> ReloadLoggedInCoachAsync(string hashedUserId, string token, CancellationToken cancellationToken);

    public Task<UpdateResult?> UpdateCoLastActive(string loggedInCoachId, CancellationToken cancellationToken);
}