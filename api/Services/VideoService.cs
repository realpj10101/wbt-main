
namespace api.Services;

public class VideoService : IVideoService
{
    private const string wwwRootPath = "wwwroot/";
    private const string videoRoot = "storage/videos";

    public async Task<string?> SaveVideoToDiskAsync(IFormFile videoFile, ObjectId userId)
    {
        if (videoFile is null || videoFile.Length == 0)
            return null;

        string userFolder = Path.Combine(videoRoot, userId.ToString(), "original");
        string absolutePath = Path.Combine(wwwRootPath, userFolder);
        Directory.CreateDirectory(absolutePath);

        string fileName = Path.GetFileName(videoFile.FileName);
        string filePath = Path.Combine(absolutePath, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await videoFile.CopyToAsync(stream);

        return Path.Combine(userFolder, fileName).Replace("\\", "/");
    }

    public bool DeleteVideoFromDisk(string relativePath)
    {
        throw new NotImplementedException();
    }
}