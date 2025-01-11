namespace api.Interfaces.Player;

public interface IRegisterPlayerRepository
{
    public Task<LoggedInDto> RegisterPlayerAsync(RegisterPlayerDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto> LoginAsync(LoginDto userInput, CancellationToken cancellationToken);
    public Task<LoggedInDto?> ReloadLoggedInUserAsync(string hashedUserId, string token, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdateLastActive(string loggedInUserId, CancellationToken cancellationToken);
}