using api.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace api.Repositories;

public class CoachUserRepository : ICoachUserRepository
{
    private readonly IMongoCollection<RootModel> _collection; // add private readonly
    private readonly ILogger<CoachUserRepository> _logger;
    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;

    public CoachUserRepository(IMongoClient client, MongoDbSettings dbSettings, IPhotoService photoService, ILogger<CoachUserRepository> logger, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionCoaches);
        _photoService = photoService;
        _logger = logger;
        _tokenService = tokenService;
    }

    public async Task<RootModel?> GetByCoachIdAsync(ObjectId coachId, CancellationToken cancellationToken)
    {
        RootModel rootModel = await _collection.Find<RootModel>(coach
            => coach.Id == coachId).SingleOrDefaultAsync(cancellationToken);

        if (rootModel is null) return null;

        return rootModel;
    }

    public async Task<RootModel?> GetByCoUserNameAsync(string userName, CancellationToken cancellationToken) =>
        await _collection.Find<RootModel>(rootModel => rootModel.NormalizedUserName == userName.ToUpper().Trim()).FirstOrDefaultAsync(cancellationToken);

    public async Task<ObjectId?> GetCoObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        ObjectId? coachId = await _collection.AsQueryable<RootModel>()
            .Where(rootModel => rootModel.NormalizedUserName == userName.ToUpper())
            .Select(item => item.Id)
            .SingleOrDefaultAsync(cancellationToken);

        return coachId is null || !coachId.HasValue ? null : coachId;
    }

    public async Task<UpdateResult?> UpdateCoachAsync(CoachUpdateDto coachUpdateDto, string? hashedUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (coachId is null) return null;

        UpdateDefinition<RootModel> updateCoach = Builders<RootModel>.Update
        .Set(coach => coach.LookingFor, coachUpdateDto.LookingFor?.Trim())
        .Set(coach => coach.City, coachUpdateDto.City?.Trim().ToLower())
        .Set(coach => coach.Country, coachUpdateDto.Country?.Trim().ToLower())
        .Set(coach => coach.Records, coachUpdateDto.Record?.Trim());

        return await _collection.UpdateOneAsync<RootModel>(coach => coach.Id == coachId, updateCoach, null, cancellationToken);
    }

    public async Task<Photo?> UploadCoachPhotoAsync(IFormFile file, string? hashedUserId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (coachId is null) return null;

        RootModel? coach = await GetByCoachIdAsync(coachId.Value, cancellationToken);
        if (coach is null)
        {
            _logger.LogError("coach is null / not found");
            return null;
        }

        string[]? imageUrls = await _photoService.AddPhotoToDisk(file, coachId.Value);

        if (imageUrls is not null)
        {
            Photo photo;

            photo = coach.Photos.Count == 0
                ? CoachMappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: true)
                : CoachMappers.ConvertPhotoUrlsToPhoto(imageUrls, isMain: false);

            coach.Photos.Add(photo);

            var updateCoach = Builders<RootModel>.Update
                .Set(doc => doc.Photos, coach.Photos);

            UpdateResult result = await _collection.UpdateOneAsync<RootModel>(doc => doc.Id == coachId, updateCoach, null, cancellationToken);

            return result.ModifiedCount == 1 ? photo : null;
        }

        _logger.LogError("PhotoService saving photo to disk failed.");
        return null;
    }

    public async Task<UpdateResult?> SetMainCoachPhotoAsync(string? hashedUserId, string photoUrlIn, CancellationToken cancellationToken)
    {
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (coachId is null) return null;

        FilterDefinition<RootModel>? filterOld = Builders<RootModel>.Filter
            .Where(coach =>
                coach.Id == coachId && coach.Photos.Any<Photo>(photo => photo.IsMain == true));

        UpdateDefinition<RootModel>? updateOld = Builders<RootModel>.Update
            .Set(coach => coach.Photos.FirstMatchingElement().IsMain, false);

        await _collection.UpdateOneAsync(filterOld, updateOld, null, cancellationToken);

        FilterDefinition<RootModel>? filterNew = Builders<RootModel>.Filter
            .Where(coach =>
                coach.Id == coachId && coach.Photos.Any<Photo>(photo => photo.Url_165 == photoUrlIn));

        UpdateDefinition<RootModel>? updateNew = Builders<RootModel>.Update
            .Set(coach => coach.Photos.FirstMatchingElement().IsMain, true);

        return await _collection.UpdateOneAsync(filterNew, updateNew, null, cancellationToken);
    }

    public async Task<UpdateResult?> DeleteCoachPhotoAsync(string? coachId, string? url_165_In, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(coachId) || string.IsNullOrEmpty(url_165_In)) return null;

        Photo photo = await _collection.AsQueryable()
        .Where(coach => coach.Id.ToString() == coachId)
        .SelectMany(coach => coach.Photos)
        .Where(photo => photo.Url_165 == url_165_In)
        .FirstOrDefaultAsync(cancellationToken);

        if (photo is null) return null;

        if (photo.IsMain) return null;

        bool isDeleteSuccess = await _photoService.DeletePhotoFormDisk(photo);
        if (!isDeleteSuccess)
        {
            _logger.LogError("Delete from disk failed.");
            return null;
        }

        var update = Builders<RootModel>.Update
            .PullFilter(coach => coach.Photos, photo => photo.Url_165 == url_165_In);

        return await _collection.UpdateOneAsync<RootModel>(coach => coach.Id.ToString() == coachId, update, null, cancellationToken);
    }
}
