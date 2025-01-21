namespace api.Interfaces;

public interface IPhotoService
{
    public Task<string[]?> AddPhotoToDisk(IFormFile file, ObjectId playerId);

    public Task<bool> DeletePhotoFromDisk(Photo photo);
}
