namespace api.Interfaces;

public interface ICoachUserRepository
{
    public Task<RootModel?> GetByCoachIdAsync(ObjectId coachId, CancellationToken cancellationToken);

    public Task<RootModel?> GetByCoUserNameAsync(string userName, CancellationToken cancellationToken);

    public Task<ObjectId?> GetCoObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken);

    public Task<UpdateResult?> UpdateCoachAsync(CoachUpdateDto coachUpdateDto, string? hashedUserId, CancellationToken cancellationToken);

    public Task<Photo?> UploadCoachPhotoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken);

    public Task<UpdateResult?> SetMainCoachPhotoAsync(string? hashedUserId, string photoUrlIn, CancellationToken cancellationToken);

    public Task<UpdateResult?> DeleteCoachPhotoAsync(string? hashedUserId, string? urlIn, CancellationToken cancellationToken);
}
