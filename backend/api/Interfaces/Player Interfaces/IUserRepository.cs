namespace api.Interfaces;

public interface IUserRepository
{
    public Task<RootModel?> GetByIdAsync(ObjectId playerId, CancellationToken cancellationToken);

    public  Task<RootModel?> GetByUserNameAsync(string userName, CancellationToken cancellationToken);

    public Task<ObjectId?> GetObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken);

    public Task<UpdateResult?> UpdatePlayerAsync(PlayerUpdateDto playerUpdateDto, string? hashedUserId, CancellationToken cancellationToken);

    public Task<Photo?> UploadPhotoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken);

    public Task<UpdateResult?> SetMainPhotoAsync(string hashedUserId, string urlIn, CancellationToken cancellationToken);

    public Task<UpdateResult?> DeletePhotoAsync(string hashedUserId, string? urlIn, CancellationToken cancellationToken);
}
