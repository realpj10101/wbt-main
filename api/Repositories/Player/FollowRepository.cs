using api.Extensions;
using api.Interfaces.Player;
using api.Models.Helpers;

namespace api.Repositories.Player;

public class FollowRepository : IFollowRepository
{
    #region Db and vars

    private readonly IMongoClient _client;
    private readonly IMongoCollection<Follow> _collection;
    private readonly ITokenService _tokenService;
    private readonly IMongoCollection<AppUser> _collectionUsers;
    private readonly IPlayerUserRepository _playerUserRepository;
    private readonly ILogger<FollowRepository> _logger;

    public FollowRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService,
        IPlayerUserRepository playerUserRepository, ILogger<FollowRepository> logger)
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Follow>(AppVariablesExtensions.collectionFollows);
        _collectionUsers = dbName.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        
        _tokenService = tokenService;
        
        _playerUserRepository = playerUserRepository;
        
        _logger = logger;
    }
    #endregion

    /// <summary>
    /// Follow the target player by username and only the logged in user can follow
    /// </summary>
    public async Task<FollowStatus> CreateAsync(ObjectId playerId, string targetPlayeUserName,
        CancellationToken cancellationToken)
    {
        FollowStatus fS = new();

        ObjectId? followedId =
            await _playerUserRepository.GetObjectIdByUserNameAsync(targetPlayeUserName, cancellationToken);

        if (followedId is null)
        {
            fS.IsTargetMemberNotFound = true;

            return fS;
        }

        if (playerId == followedId)
        {
            fS.IsFollowingThemself = true;

            return fS;
        }

        bool isFollowing = await _collection.Find<Follow>(followDoc =>
                followDoc.FollowerId == playerId &&
                followDoc.FollowedMemberId == followedId)
                .AnyAsync(cancellationToken);

        if (isFollowing)
        {
            fS.IsAlreadyFollowed = true;

            return fS;
        }

        Follow follow = Mappers.ConvertFollowIdsToFollow(playerId, followedId.Value);
        
        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);
        
        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, follow, null, cancellationToken);

            #region UpdateCounters

            UpdateDefinition<AppUser> updateFollowingsCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.FollowingsCount, 1); // Increment by 1 for each follow

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == playerId, updateFollowingsCount, null, cancellationToken);

            UpdateDefinition<AppUser> updateFollowersCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.FollowersCount, 1); // Increment by 1 for each follow

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == followedId, updateFollowersCount, null, cancellationToken);

            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            fS.IsSuccess = true;
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "Follow failed."
                + "MESSAGE:" + ex.Message
                + "TRACE:" + ex.StackTrace
            );
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session finished.");
        }

        return fS;
    }

    /// Unfollow the target player by logged in user
    public async Task<FollowStatus> DeleteAsync(ObjectId playerId, string followedPlayerUserName,
        CancellationToken cancellationToken)
    {
        FollowStatus fS = new();

        ObjectId? followedId = // Get target player username and find his/her ObjectId.
            await _playerUserRepository.GetObjectIdByUserNameAsync(followedPlayerUserName, cancellationToken);

        if (followedId is null)
        {
            fS.IsTargetMemberNotFound = true;

            return fS;
        }

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);
        
        session.StartTransaction();

        try
        {
            DeleteResult deleteResult = await _collection.DeleteOneAsync(
                doc => doc.FollowerId == playerId
                       && doc.FollowedMemberId == followedId,
                cancellationToken);

            if (deleteResult.DeletedCount < 1)
            {
                fS.IsAlreadyUnfollowed = true;

                return fS;
            }

            #region UpdateCounters

            UpdateDefinition<AppUser> updateFollowingsCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.FollowingsCount, -1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == playerId, updateFollowingsCount, null, cancellationToken);

            UpdateDefinition<AppUser> updateFollowersCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.FollowersCount, -1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == followedId, updateFollowersCount, null, cancellationToken);

            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            fS.IsSuccess = true;
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "UnFollow failed."
                + "MESSAGE:" + ex.Message
                + "TRACE:" + ex.StackTrace);
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session finished.");
        }

        return fS;
    }

    public async Task<bool> CheckIsFollowingAsync(ObjectId playerId, AppUser appUser,
        CancellationToken cancellationToken) => 
        await _collection.Find<Follow>(
            doc => doc.FollowerId == playerId && doc.FollowedMemberId == appUser.Id
            ).AnyAsync(cancellationToken);
}