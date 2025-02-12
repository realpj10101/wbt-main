namespace api.Repositories.Player;

public interface IPlayerUserRepository
{
    public Task<AppUser?> GetByIdAsync(ObjectId playerId, CancellationToken cancellationToken);
    public Task<AppUser?> GetPlayerByNameAsync(string userName, CancellationToken cancellationToken);
    public Task<ObjectId?> GetObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken);
    public Task<UpdateResult?> UpdatePlayerAsync(PlayerUpdateDto playerUpdateDto, string? hashedUserId, CancellationToken cancellationToken);
    public Task<Photo?> UploadPhotoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken);
    public Task<UpdateResult?> SetMainPhotoAsync(string hashedUserId, string photoUrlIn, CancellationToken cancellationToken);
    public Task<UpdateResult?> DeletePhotoAsync(string hashedUserId, string? urlIn, CancellationToken cancellationToken);
}