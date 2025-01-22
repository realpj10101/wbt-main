using api.Extensions;
using api.Interfaces.Player;

namespace api.Repositories.Player;

public class PlayerDetailsRepository : IPlayerDetailsRepository
{
    #region Constructor

    private readonly IMongoCollection<AppUser> _collection;
    private readonly ILogger<PlayerDetailsRepository> _logger;
    private readonly ITokenService _tokenService;

    public PlayerDetailsRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings,
        ILogger<PlayerDetailsRepository> logger, ITokenService tokenService,
        IPhotoService photoService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        _logger = logger;
        _tokenService = tokenService;
    }

    #endregion

    public async Task<UpdateResult?> UpdatePlayerDetailsAsync(PlayerDetailsDto playerDetailsDto, string? hashedUseId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(hashedUseId)) return null;

        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUseId, cancellationToken);

        if (playerId is null) return null;

        UpdateDefinition<AppUser> updatePlayer = Builders<AppUser>.Update
            .Set(appUser => appUser.Name, playerDetailsDto.Name)
            .Set(appUser => appUser.LastName, playerDetailsDto.LastName)
            .Set(appUser => appUser.NationalCode, playerDetailsDto.NationalCode)
            .Set(appUser => appUser.Height, playerDetailsDto.Height)
            .Set(appUser => appUser.KnownAs, playerDetailsDto.KnownAs)
            .Set(appUser => appUser.LookingFor, playerDetailsDto.LookingFor)
            .Set(appUser => appUser.Records, playerDetailsDto.Records)
            .Set(appUser => appUser.City, playerDetailsDto.City)
            .Set(appUser => appUser.Country, playerDetailsDto.Country);

        return await _collection.UpdateOneAsync<AppUser>(appUser => appUser.Id == playerId, updatePlayer, null,
            cancellationToken);
    }
}