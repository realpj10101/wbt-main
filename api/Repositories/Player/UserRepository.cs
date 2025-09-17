using api.Enums;
using api.Extensions;
using api.Helpers;

namespace api.Repositories.Player;

public class UserRepository : IUserRepository
{

    #region Constructor

    private readonly IMongoCollection<AppUser> _collection;
    private readonly ILogger<UserRepository> _logger;
    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;
    private readonly IVideoService _videoService;

    public UserRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings,
        ILogger<UserRepository> logger, ITokenService tokenService,
        IPhotoService photoService, IVideoService videoService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.CollectionUsers);
        _logger = logger;
        _tokenService = tokenService;
        _photoService = photoService;
        _videoService = videoService;
    }

    #endregion

    #region User Management

    public async Task<AppUser?> GetByIdAsync(ObjectId playerId, CancellationToken cancellationToken)
    {
        AppUser? appUser = await _collection.Find<AppUser>(player
            => player.Id == playerId).SingleOrDefaultAsync(cancellationToken);

        if (appUser is null) return null;

        return appUser;
    }

    public async Task<AppUser?> GetPlayerByNameAsync(string userName, CancellationToken cancellationToken) =>
        await _collection.Find<AppUser>(appUser => appUser.NormalizedUserName == userName.ToUpper().Trim()).SingleOrDefaultAsync(cancellationToken);

    public async Task<ObjectId?> GetObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _collection.AsQueryable<AppUser>()
            .Where(appUser => appUser.NormalizedUserName == userName.ToUpper())
            .Select(item => item.Id)
            .SingleOrDefaultAsync(cancellationToken);

        return ValidationsExtensions.TestValidateObjectId(playerId);
    }

    public async Task<UpdateResult?> UpdatePlayerAsync(UserUpdateDto userUpdateDto, string? hashedUserId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        var updates = new List<UpdateDefinition<AppUser>>();

        if (!string.IsNullOrWhiteSpace(userUpdateDto.Name))
            updates.Add(Builders<AppUser>.Update.Set(u => u.Name, userUpdateDto.Name.Trim()));

        if (!string.IsNullOrWhiteSpace(userUpdateDto.LastName))
            updates.Add(Builders<AppUser>.Update.Set(u => u.LastName, userUpdateDto.LastName.Trim()));

        if (userUpdateDto.Height.HasValue)
            updates.Add(Builders<AppUser>.Update.Set(u => u.Height, userUpdateDto.Height.Value));

        if (userUpdateDto.Weight.HasValue)
            updates.Add(Builders<AppUser>.Update.Set(u => u.Weight, userUpdateDto.Weight.Value));

        if (!string.IsNullOrWhiteSpace(userUpdateDto.Gender))
            updates.Add(Builders<AppUser>.Update.Set(u => u.Gender, userUpdateDto.Gender.Trim()));

        if (!string.IsNullOrWhiteSpace(userUpdateDto.Position))
        {
            if (Enum.TryParse<PositionsEnum>(userUpdateDto.Position.Trim(), true, out var positionEnum))
            {
                updates.Add(Builders<AppUser>.Update.Set(u => u.Position, positionEnum));
            }
            else
            {
                // Optional: log or handle invalid string case
                throw new ArgumentException($"Invalid position: {userUpdateDto.Position}");
            }
        }
        
        if (!string.IsNullOrWhiteSpace(userUpdateDto.ExperienceLevel))
            updates.Add(Builders<AppUser>.Update.Set(u => u.ExperienceLevel, userUpdateDto.ExperienceLevel.Trim()));

        if (!string.IsNullOrWhiteSpace(userUpdateDto.Skills))
            updates.Add(Builders<AppUser>.Update.Set(u => u.Skills, userUpdateDto.Skills.Trim()));
        
        if (userUpdateDto.GamesPlayed.HasValue)
            updates.Add(Builders<AppUser>.Update.Set(u => u.GamesPlayed, userUpdateDto.GamesPlayed.Value));
        
        if (userUpdateDto.PointsPerGame.HasValue)
            updates.Add(Builders<AppUser>.Update.Set(u => u.PointsPerGame, userUpdateDto.PointsPerGame.Value));
        
        if (userUpdateDto.ReboundsPerGame.HasValue)
            updates.Add(Builders<AppUser>.Update.Set(u => u.ReboundsPerGame, userUpdateDto.ReboundsPerGame.Value));
        
        if (userUpdateDto.AssistsPerGame.HasValue)
            updates.Add(Builders<AppUser>.Update.Set(u => u.AssistsPerGame, userUpdateDto.AssistsPerGame.Value));
        
        if (!string.IsNullOrWhiteSpace(userUpdateDto.Bio))
            updates.Add(Builders<AppUser>.Update.Set(u => u.Bio, userUpdateDto.Bio.Trim()));
        
        if (!string.IsNullOrWhiteSpace(userUpdateDto.Achievements))
            updates.Add(Builders<AppUser>.Update.Set(u => u.Achievements, userUpdateDto.Achievements.Trim()));
        
        if (!string.IsNullOrWhiteSpace(userUpdateDto.City))
            updates.Add(Builders<AppUser>.Update.Set(u => u.City, userUpdateDto.City.Trim()));
        
        if (!string.IsNullOrWhiteSpace(userUpdateDto.Region))
            updates.Add(Builders<AppUser>.Update.Set(u => u.Region, userUpdateDto.Region.Trim()));

        if (!string.IsNullOrWhiteSpace(userUpdateDto.Country))
            updates.Add(Builders<AppUser>.Update.Set(u => u.Country, userUpdateDto.Country.Trim()));
        
        var updateDef = Builders<AppUser>.Update.Combine(updates);

        if (!updates.Any())
            return null; // or return early if nothing to update

        return await _collection.UpdateOneAsync<AppUser>(
            appUser => appUser.Id == playerId,
            updateDef,
            null,
            cancellationToken);
    }

    public async Task<string?> GetUserNameByIdentifierHashAsync(
        string identifierHash, CancellationToken cancellationToken
    ) =>
        await _collection.AsQueryable().Where(appUser => appUser.IdentifierHash == identifierHash)
            .Select(appUser => appUser.NormalizedUserName).SingleOrDefaultAsync(cancellationToken);

    public Task<AppUser?> GetByIdentifierHashAsync(string identifierHash, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Photo Managemnet

    public async Task<Photo?> UploadPhotoAsync(IFormFile file, string? hashedUserId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        AppUser? appUser = await GetByIdAsync(playerId.Value, cancellationToken);
        if (appUser is null)
        {
            _logger.LogError("appUser is null / not found");

            return null;
        }

        // userId, appUser, file
        // save file in Storage using PhotoService / userId makes the folder name
        string[]? imageUrls = await _photoService.AddPhotoToDiskAsync(file, playerId.Value);

        if (imageUrls is not null)
        {
            Photo photo;

            photo = appUser.Photos.Count == 0
                ? Mappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: true)
                : Mappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: false);

            appUser.Photos.Add(photo);

            var updatePlayer = Builders<AppUser>.Update
                .Set(doc => doc.Photos, appUser.Photos);

            UpdateResult result =
                await _collection.UpdateOneAsync<AppUser>(doc => doc.Id == playerId, updatePlayer, null, cancellationToken);

            return result.ModifiedCount == 1 ? photo : null;
        }

        _logger.LogError("PhotoService saving photo to disk failed");
        return null;
    }

    public async Task<UpdateResult?> SetMainPhotoAsync(string hashedUserId, string photoUrlIn,
        CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        #region Unset the previous main photo: Find the photo with IsMain True; update IsMain to False

        FilterDefinition<AppUser>? filterOld = Builders<AppUser>.Filter
            .Where(appUser =>
                appUser.Id == playerId && appUser.Photos.Any<Photo>(photo => photo.IsMain == true));

        UpdateDefinition<AppUser>? updateOld = Builders<AppUser>.Update
            .Set(appUser => appUser.Photos.FirstMatchingElement().IsMain, false);

        await _collection.UpdateOneAsync(filterOld, updateOld, null, cancellationToken);

        #endregion  

        #region SET the new main photo: find new photo by its Url_165; update IsMain to True

        FilterDefinition<AppUser>? filterNew = Builders<AppUser>.Filter
            .Where(appUser =>
                appUser.Id == playerId && appUser.Photos.Any<Photo>(photo => photo.Url_165 == photoUrlIn));

        UpdateDefinition<AppUser>? updateNew = Builders<AppUser>.Update
            .Set(appUser => appUser.Photos.FirstMatchingElement().IsMain, true);

        return await _collection.UpdateOneAsync(filterNew, updateNew, null, cancellationToken);

        #endregion
    }

    public async Task<UpdateResult?> DeletePhotoAsync(string hashedUserId, string? url_165_In,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(url_165_In)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        Photo photo = await _collection.AsQueryable()
            .Where(appUser => appUser.Id == playerId) // filter by user email
            .SelectMany(appUser => appUser.Photos) // flatten the photos array
            .Where(photo => photo.Url_165 == url_165_In) // filter by photo url
            .FirstOrDefaultAsync(cancellationToken); // return the photo url

        if (photo is null) return null; // Warning: should be handled with Exception handling Middlewear to log the app's bug since it's a bug

        if (photo.IsMain) return null; // prevent from deleting main photo!

        bool isDeleteSuccess = await _photoService.DeletePhotoFromDisk(photo);
        if (!isDeleteSuccess)
        {
            _logger.LogError("Delete Photo form disk failed");

            return null;
        }

        var update = Builders<AppUser>.Update
            .PullFilter(appUser => appUser.Photos, photo => photo.Url_165 == url_165_In);

        return await _collection.UpdateOneAsync<AppUser>(appUser => appUser.Id == playerId, update, null, cancellationToken);
    }

    public async Task<Video?> UploadVideoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? userId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);
        
        if (userId is null) return null;

        AppUser? appUser = await GetByIdAsync(userId.Value, cancellationToken);
        if (appUser is null)
        {
            _logger.LogError("AppUser is null");

            return null;
        }
        
        string? videoUrl = await _videoService.SaveVideoToDiskAsync(file, userId.Value);
        if (videoUrl is not null)
        {
            Video? video;
            
            video = Mappers.ConvertVideoUrlToVideo(videoUrl);
            
            appUser.Videos.Add(video);
            
            var updatedUser = Builders<AppUser>.Update
                .Set(doc => doc.Videos, appUser.Videos);
            
            UpdateResult updateResult = await 
                _collection.UpdateOneAsync(doc => doc.Id == userId, updatedUser, null, cancellationToken);
            
            return updateResult.ModifiedCount == 1 ? video : null;
        }
        
        _logger.LogError("VideoService saving video to disk failed");
        return null;
    }

    #endregion
}