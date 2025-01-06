namespace api.Repositories.Player;

public interface IPlayerUserRepository
{
    public Task<ObjectId?> GetObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken);
}