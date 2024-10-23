using api.DTOs.Team;

namespace api.Interfaces.Team;

public interface ITeamUserRepository
{
    public Task<RootModel?> GetByIdAsync(ObjectId teamId, CancellationToken cancellationToken);

    public Task<RootModel?> GetByTeamUserNameAsync(string userName, CancellationToken cancellationToken);
    
    public Task<ObjectId?> GetObjectIdByTeamUserNameAsync(string userName, CancellationToken cancellationToken);

    public Task<UpdateResult?> UpdateTeamAsync(TeamUpdateDto teamUpdateDto, string? hashedUserId, CancellationToken cancellationToken);

    public Task<Photo?> UploadTeamPhotoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken);

    public Task<UpdateResult?> SetTeamMainPhotoAsync(string hashedUserId, string urlIn, CancellationToken cancellationToken);

    public Task<UpdateResult?> DeleteTeamPhotoAsync(string hashedUserId, string? urlIn, CancellationToken cancellationToken);
}