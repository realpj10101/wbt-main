using api.Extensions;

namespace api.Repositories.Player;

public class PlayerUserRepository : IPlayerUserRepository
{
    #region Constructor
    
    private readonly IMongoCollection<AppUser> _collection;
    private readonly ILogger<PlayerUserRepository> _logger;
    private readonly ITokenService _tokenService;

    public PlayerUserRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings,
        ILogger<PlayerUserRepository> logger, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        _logger = logger;
        _tokenService = tokenService;
    }
    
    #endregion

    #region User Management

    public async Task<ObjectId?> GetObjectIdByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _collection.AsQueryable<AppUser>()
            .Where(appUser => appUser.NormalizedUserName == userName.ToUpper())
            .Select(item => item.Id)
            .SingleOrDefaultAsync(cancellationToken);
        
        return ValidationsExtensions.ValidateObjectId(playerId);
    }

    #endregion
}