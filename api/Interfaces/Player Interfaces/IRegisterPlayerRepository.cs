namespace api.Interfaces;
    public interface IRegisterPlayerRepository
{
    public Task<LoggedInDto> CreateAsync(RegisterPlayerDto playerInput, CancellationToken cancellationToken);

    public Task<LoggedInDto> LoginAsync(LoginDto playerInput, CancellationToken cancellationToken);

    public Task<LoggedInDto?> ReloadLoggedInPlayerAsync(string hashedUserId, string token, CancellationToken cancellationToken);

    public Task<UpdateResult?> UpdateLastActive(string loggedInPlayerId, CancellationToken cancellationToken);
}