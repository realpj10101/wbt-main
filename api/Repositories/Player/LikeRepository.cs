using api.Extensions;
using api.Interfaces.Player;
using api.Models.Helpers;

namespace api.Repositories.Player;

public class LikeRepository : ILikeRepository
{
    #region DB and vars

    private readonly IMongoClient _client;
    private readonly IMongoCollection<Like> _collection;
    private readonly ITokenService _tokenService;
    private readonly IMongoCollection<AppUser> _collectionUsers;
    private readonly IPlayerUserRepository _playerUserRepository;
    private readonly ILogger<LikeRepository> _logger;

    public LikeRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService,
        IPlayerUserRepository playerUserRepository, ILogger<LikeRepository> logger)
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Like>(AppVariablesExtensions.collectionLikes);
        _collectionUsers = dbName.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        
        _tokenService = tokenService;
        
        _playerUserRepository = playerUserRepository;
        
        _logger = logger;
    }
    #endregion
    
    
    // Add like 
    public async Task<LikeStatus> CreateAsync(ObjectId playerId, string targetMemberUserName,
        CancellationToken cancellationToken)
    {
        LikeStatus lS = new();

        ObjectId? targetId =
            await _playerUserRepository.GetObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

        if (targetId is null)
        {
            lS.IsTargetMemberNotFound = true;

            return lS;
        }

        bool isFollowing = await _collection.Find<Like>(likeDoc =>
                likeDoc.LikerId == playerId &&
                likeDoc.LikedMemberId == targetId)
                .AnyAsync(cancellationToken);
        
        if (isFollowing)
        {
            lS.IsAlreadyLiked = true;

            return lS;
        }

        Like like = Mappers.ConvertLikeIdsToLike(playerId, targetId.Value);

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);
        
        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, like, null, cancellationToken);

            #region UpdateCounters

            UpdateDefinition<AppUser> updateCommentingsCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.LikingsCount, 1); // Increment by 1 for each like
            
            await _collectionUsers.UpdateOneAsync()

            #endregion
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}