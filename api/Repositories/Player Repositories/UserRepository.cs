using api.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<RootModel> _collection;
    private readonly ILogger<UserRepository> _logger;

    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;

    public UserRepository(IMongoClient client, MongoDbSettings dbSettings, IPhotoService photoService, ILogger<UserRepository> logger, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionPlayers);
        _photoService = photoService;
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task<RootModel?> GetByIdAsync(ObjectId playerId, CancellationToken cancellationToken)
    {
        RootModel? rootModel = await _collection.Find<RootModel>(player
            => player.Id == playerId).SingleOrDefaultAsync(cancellationToken);

        if (rootModel is null) return null;

        return rootModel;
    }

    public async Task<RootModel?> GetByUserNameAsync(string userName, CancellationToken cancellationToken) =>
      await _collection.Find<RootModel>(rootModel => rootModel.UserName == userName.ToUpper().Trim()).FirstOrDefaultAsync(cancellationToken);

    public async Task<ObjectId?> GetObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _collection.AsQueryable<RootModel>()
        .Where(rootModel => rootModel.NormalizedUserName == userName.ToUpper())
        .Select(item => item.Id)
        .SingleOrDefaultAsync(cancellationToken);

        return ValidationsExtensions.ValidateObjectId(playerId);
    }

    public async Task<UpdateResult?> UpdatePlayerAsync(PlayerUpdateDto playerUpdateDto, string? hashedUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        UpdateDefinition<RootModel> updatedPlayer = Builders<RootModel>.Update
        .Set(player => player.Height, playerUpdateDto.Height)
        .Set(player => player.KnownAs, playerUpdateDto.KnownAs?.Trim())
        .Set(player => player.LookingFor, playerUpdateDto.LookingFor?.Trim())
        .Set(player => player.City, playerUpdateDto.City?.Trim().ToLower())
        .Set(player => player.Country, playerUpdateDto.Country?.Trim().ToLower());

        return await _collection.UpdateOneAsync<RootModel>(player => player.Id == playerId, updatedPlayer, null, cancellationToken);
    }

    public async Task<Photo?> UploadPhotoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        RootModel? player = await GetByIdAsync(playerId.Value, cancellationToken);

        if (player is null)
        {
            _logger.LogError("player is null / not found");
            return null;
        }

        string[]? imageUrls = await _photoService.AddPhotoToDisk(file, playerId.Value);

        if (imageUrls is not null)
        {
            Photo photo;

            photo = player.Photos.Count == 0
                ? Mappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: true)
                : Mappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: false);

            player.Photos.Add(photo);

            var updatedPlayer = Builders<RootModel>.Update
                .Set(doc => doc.Photos, player.Photos);

            UpdateResult result = await _collection.UpdateOneAsync<RootModel>(doc => doc.Id == playerId, updatedPlayer, null, cancellationToken);

            return result.ModifiedCount == 1 ? photo : null;
        }

        _logger.LogError("PhotoService saving photo to disk failed");
        return null;
    }

    public async Task<UpdateResult?> SetMainPhotoAsync(string hashedUserId, string photoUrlIn, CancellationToken cancellationToken) 
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        #region UNSET the previous main photo: Find the photo with IsMain True; update IsMain to False
        FilterDefinition<RootModel>? filterOld = Builders<RootModel>.Filter
            .Where(player =>
                    player.Id == playerId && player.Photos.Any<Photo>(photo => photo.IsMain == true));

        UpdateDefinition<RootModel>? updateOld = Builders<RootModel>.Update
            .Set(player => player.Photos.FirstMatchingElement().IsMain, false);

        await _collection.UpdateOneAsync(filterOld, updateOld, null, cancellationToken);
        #endregion

        #region SET the new main photo: find new photo by its Url_165; update IsMain to True
        FilterDefinition<RootModel>? filterNew = Builders<RootModel>.Filter
            .Where(player =>
                     player.Id == playerId && player.Photos.Any<Photo>(photo => photo.Url_165 == photoUrlIn));

        UpdateDefinition<RootModel>? updateNew = Builders<RootModel>.Update
            .Set(player => player.Photos.FirstMatchingElement().IsMain, true);

        return await _collection.UpdateOneAsync(filterNew, updateNew, null, cancellationToken);
        #endregion
    }

    public async Task<UpdateResult?> DeletePhotoAsync(string hashedUserId, string? url_165_In, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(url_165_In)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        Photo photo = await _collection.AsQueryable()
        .Where(player => player.Id == playerId)
        .SelectMany(player => player.Photos)
        .Where(photo => photo.Url_165 == url_165_In)
        .FirstOrDefaultAsync(cancellationToken);

        if (photo is null) return null;

        if (photo.IsMain) return null;

        bool isDeleteSucceess = await _photoService.DeletePhotoFormDisk(photo);
        if (!isDeleteSucceess)
        {
            _logger.LogError("Delete from disk failed.");
            return null;
        }

        var update = Builders<RootModel>.Update
            .PullFilter(player => player.Photos, photo => photo.Url_165 == url_165_In);

        return await _collection.UpdateOneAsync<RootModel>(player => player.Id == playerId, update, null, cancellationToken);
    }
}
