using api.DTOs.Team;
using api.Extensions;
using api.Interfaces.Team;
using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace api.Repositories.Team;

public class TeamUserRepository : ITeamUserRepository
{
    private readonly IMongoCollection<RootModel> _collection;
    private readonly ILogger<TeamUserRepository> _logger;

    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;

    public TeamUserRepository(IMongoClient client, MongoDbSettings dbSettings, IPhotoService photoService, ILogger<TeamUserRepository> logger, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionTeams);
        _photoService = photoService;
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task<RootModel?> GetByIdAsync(ObjectId teamId, CancellationToken cancellationToken)
    {
        RootModel? rootModel = await _collection.Find<RootModel>(team
            => team.Id == teamId).SingleOrDefaultAsync(cancellationToken);

        if (rootModel is null) return null;

        return rootModel;
    }

    public async Task<RootModel?> GetByTeamUserNameAsync(string userName, CancellationToken cancellationToken) =>
        await _collection.Find<RootModel>(rootModel => rootModel.UserName ==  userName.ToUpper().Trim()).FirstOrDefaultAsync(cancellationToken);

    public async Task<ObjectId?> GetObjectIdByTeamUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        ObjectId? teamId = await _collection.AsQueryable<RootModel>()
        .Where(rootModel => rootModel.NormalizedUserName == userName.ToUpper())
        .Select(item => item.Id)
        .SingleOrDefaultAsync(cancellationToken);

        return ValidationsExtensions.ValidateObjectId(teamId);
    }

    public async Task<UpdateResult?> UpdateTeamAsync(TeamUpdateDto teamUpdateDto, string? hashedUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? teamId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        UpdateDefinition<RootModel> updatedTeam = Builders<RootModel>.Update
        .Set(team => team.NumberOfGames, teamUpdateDto.NumberOfGames)
        .Set(team => team.NumberOfWins, teamUpdateDto.NumberOfWins)
        .Set(team => team.NumberOfLosses, teamUpdateDto.NumberOfLosses)
        .Set(team => team.GameHistory, teamUpdateDto.GameHistory)
        .Set(team => team.GameResults, teamUpdateDto.GameResults);

        return await _collection.UpdateOneAsync<RootModel>(team => team.Id == teamId, updatedTeam, null, cancellationToken);
    }

    public async Task<Photo?> UploadTeamPhotoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? teamId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (teamId is null) return null;

        RootModel? team = await GetByIdAsync(teamId.Value, cancellationToken);

        if (team is null)
        {
            _logger.LogError("team is null / not foud");
            return null;
        }

        string[]? imageUrls = await _photoService.AddPhotoToDisk(file, teamId.Value);

        if (imageUrls is not null)
        {
            Photo photo;

            photo = team.Photos.Count == 0
                ? TeamMappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: true)
                : TeamMappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: false);

            team.Photos.Add(photo);

            var updatedTeam = Builders<RootModel>.Update
                .Set(doc => doc.Photos, team.Photos);

            UpdateResult result = await _collection.UpdateOneAsync<RootModel>(doc => doc.Id == teamId, updatedTeam, null, cancellationToken);

            return result.ModifiedCount == 1 ? photo : null;
        }

        _logger.LogError("PhotoService saving photo to dusk failed");
        return null;
    }

    public async Task<UpdateResult?> SetTeamMainPhotoAsync(string hashedUserId, string photoUrlIn, CancellationToken cancellationToken)
    {
        ObjectId? teamId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (teamId is null) return null;

        #region UNSET the previous main photo: Find the photo with IsMain True; update IsMain to False
        FilterDefinition<RootModel>? filterOld = Builders<RootModel>.Filter
            .Where(team =>
                    team.Id == teamId && team.Photos.Any<Photo>(photo => photo.IsMain == true));

        UpdateDefinition<RootModel>? updateOld = Builders<RootModel>.Update
            .Set(team => team.Photos.FirstMatchingElement().IsMain, false);

        await _collection.UpdateOneAsync(filterOld, updateOld, null, cancellationToken);
        #endregion

        #region SET the new main photo: find new photo by its Url_165; update IsMain to True
        FilterDefinition<RootModel>? filterNow = Builders<RootModel>.Filter
            .Where(team =>
                    team.Id == teamId && team.Photos.Any<Photo>(photo => photo.Url_165 == photoUrlIn));

        UpdateDefinition<RootModel>? updateNew = Builders<RootModel>.Update
            .Set(team => team.Photos.FirstMatchingElement().IsMain, true);

        return await _collection.UpdateOneAsync(filterNow, updateNew, null, cancellationToken);
        #endregion
    }

    public async Task<UpdateResult?> DeleteTeamPhotoAsync(string hashedUserId, string? url_165_In, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(url_165_In)) return null;

        ObjectId? teamId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (teamId is null) return null;

        Photo photo = await _collection.AsQueryable()
        .Where(team => team.Id == teamId)
        .SelectMany(team => team.Photos)
        .Where(photo => photo.Url_165 == url_165_In)
        .FirstOrDefaultAsync(cancellationToken);

        if (photo is null) return null;

        if (photo.IsMain) return null;

        bool isDeleteSuccess = await _photoService.DeletePhotoFormDisk(photo);
        if (!isDeleteSuccess)
        {
            _logger.LogError("Delete from disk failed");
            return null;
        }

        var update = Builders<RootModel>.Update
            .PullFilter(team => team.Photos, photo => photo.Url_165 == url_165_In);

        return await _collection.UpdateOneAsync<RootModel>(team => team.Id == teamId, update, null, cancellationToken);
    }
}