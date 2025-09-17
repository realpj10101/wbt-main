namespace api.Interfaces;

public interface IVideoService
{
    public Task<string?> SaveVideoToDiskAsync(IFormFile videoFile, ObjectId userId);
    public bool DeleteVideoFromDik(string relativePath);    
}