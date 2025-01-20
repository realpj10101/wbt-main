namespace api.Repositories.Player;

public interface IPlayerUserRepository
{
    public Task<AppUser?> GetByIdAsync(ObjectId playerId, CancellationToken cancellationToken);
    public Task<AppUser?> GetPlayerByNameAsync(string userName, CancellationToken cancellationToken);
    public Task<ObjectId?> GetObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdatePlayerAsync(PlayerUpdateDto playerUpdateDto, string? hashedUserId, CancellationToken cancellationToken);
}