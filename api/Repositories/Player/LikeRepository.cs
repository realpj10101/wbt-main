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

            UpdateDefinition<AppUser> updateLikingsCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.LikingsCount, 1); // Increment by 1 for each like

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == playerId, updateLikingsCount, null, cancellationToken);

            UpdateDefinition<AppUser> updateLikersCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.LikersCount, 1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == targetId, updateLikersCount, null, cancellationToken);

            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            lS.IsSuccess = true;
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "Like failed."
                + "MESSAGE:" + ex.Message
                + "TRACE:" + ex.StackTrace
            );
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session finished.");
        }

        return lS;
    }
    
    //dislike the target member by logged in user
    public async Task<LikeStatus> DeleteAsync(ObjectId playerId, string targetMemberUserName,
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

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);
        
        session.StartTransaction();

        try
        {
            DeleteResult deleteResult = await _collection.DeleteOneAsync(
                doc => doc.LikerId == playerId
                       && doc.LikedMemberId == targetId,
                cancellationToken);

            if (deleteResult.DeletedCount < 1)
            {
                lS.IsAlreadyDisLiked = true;

                return lS;
            }

            #region UpdateCounters

            UpdateDefinition<AppUser> updateLikingsCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.LikingsCount, -1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == playerId, updateLikingsCount, null, cancellationToken);

            UpdateDefinition<AppUser> updateLikersCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.LikersCount, -1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == targetId, updateLikersCount, null, cancellationToken);

            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            lS.IsSuccess = true;
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "Like failed."
                + "MESSAGE:" + ex.Message
                + "TRACE:" + ex.StackTrace
            );
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session finished.");
        }

        return lS;
    }
}