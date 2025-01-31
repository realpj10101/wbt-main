using api.Extensions;

namespace api.Repositories.Player;

public class PlayerUserRepository : IPlayerUserRepository
{
    
    #region Constructor
    
    private readonly IMongoCollection<AppUser> _collection;
    private readonly ILogger<PlayerUserRepository> _logger;
    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;

    public PlayerUserRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings,
        ILogger<PlayerUserRepository> logger, ITokenService tokenService,
        IPhotoService photoService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        _logger = logger;
        _tokenService = tokenService;
        _photoService = photoService;
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
        
        return ValidationsExtensions.ValidateObjectId(playerId);
    }
    
    // public async Task<UpdateResult?> UpdatePlayerAsync(PlayerUpdateDto playerUpdateDto, string? hashedUserId,
    //     CancellationToken cancellationToken)
    // {
    //     if (string.IsNullOrEmpty(hashedUserId)) return null;
    //
    //     ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);
    //
    //     if (playerId is null) return null;
    //
    //     UpdateDefinition<AppUser> updatePlayer = Builders<AppUser>.Update
    //         .Set(appUser => appUser.Height, playerUpdateDto.Height)
    //         .Set(appUser => appUser.KnownAs, playerUpdateDto.KnownAs)
    //         .Set(appUser => appUser.LookingFor, playerUpdateDto.LookingFor)
    //         .Set(appUser => appUser.City, playerUpdateDto.City)
    //         .Set(appUser => appUser.Country, playerUpdateDto.Country);
    //     
    //     return await _collection.UpdateOneAsync<AppUser>(appUser => appUser.Id == playerId, updatePlayer, null, cancellationToken);
    // }
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
        string[]? imageUrls = await _photoService.AddPhotoToDisk(file, playerId.Value);

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
                appUser.Id == playerId && appUser.Photos.Any<Photo>(photo => photo.Url_165== photoUrlIn));

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

    #endregion
}