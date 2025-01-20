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
    
    public async Task<UpdateResult?> UpdatePlayerAsync(PlayerUpdateDto playerUpdateDto, string? hashedUserId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUserId)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        UpdateDefinition<AppUser> updatePlayer = Builders<AppUser>.Update
            .Set(appUser => appUser.Height, playerUpdateDto.Height)
            .Set(appUser => appUser.KnownAs, playerUpdateDto.KnownAs)
            .Set(appUser => appUser.LookingFor, playerUpdateDto.LookingFor)
            .Set(appUser => appUser.City, playerUpdateDto.City)
            .Set(appUser => appUser.Country, playerUpdateDto.Country);
        
        return await _collection.UpdateOneAsync<AppUser>(appUser => appUser.Id == playerId, updatePlayer, null, cancellationToken);
    }
    #endregion
    
    
}