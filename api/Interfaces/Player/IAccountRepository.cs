namespace api.Interfaces.Player;

public interface IAccountRepository
{
    public Task<LoggedInDto> RegisterPlayerAsync(RegisterDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto> LoginAsync(LoginDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto?> ReloadLoggedInUserAsync(string hashedUserId, string token, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdateLastActive(string loggedInUserId, CancellationToken cancellationToken);
}