using System.Diagnostics;
using System.IO.Pipelines;
using api.Enums;
using api.Extensions;
using api.Helpers;
using api.Models.Helpers;

namespace api.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly IMongoClient _client;
    private readonly IMongoCollection<Follow> _collection;
    private readonly ITokenService _tokenService;
    private readonly IMongoCollection<RootModel> _collectionUsers;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<FollowRepository> _logger;

    public FollowRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService, IUserRepository userRepository, ILogger<FollowRepository> logger
    )
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Follow>(AppVariablesExtensions.collectionFollows);

        _tokenService = tokenService;
        _collectionUsers = dbName.GetCollection<RootModel>
        (AppVariablesExtensions.collectionPlayers);

        _userRepository = userRepository;

        _logger = logger;
    }

    public async Task<FollowStatus> CreateFollowAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken)
    {
        FollowStatus followStatus = new();

        ObjectId? followedId = await _userRepository.GetObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

        if (followedId is null)
        {
            followStatus.IsTargetMemberNotFound = true;

            return followStatus;
        }

        if (playerId == followedId)
        {
            followStatus.IsFollowingThemself = true;

            return followStatus;
        }

        bool IsFollowingAgain = await _collection.Find(followDoc => followDoc.FollowedMemberId == followedId).AnyAsync(cancellationToken);

        if (IsFollowingAgain)
        {
            followStatus.IsAlreadyFollowed = true;

            return followStatus;
        }

        Follow follow = Mappers.ConvertFollowIdsToFollow(playerId, followedId.Value);

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);

        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, follow, null, cancellationToken);

            #region UpdateCounters
            UpdateDefinition<RootModel> updateFollowingsCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowingsCount, 1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == playerId, updateFollowingsCount, null, cancellationToken);

            UpdateDefinition<RootModel> updateFollowersCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowersCount, 1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == followedId, updateFollowersCount, null, cancellationToken);
            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            followStatus.IsSuccess = true;
        }
        catch (System.Exception ex)
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
            _logger.LogInformation("MongoDB transaction/sesssion is finished");
        }

        return followStatus;
    }

    public async Task<FollowStatus> RemoveAsync(ObjectId playerId, string targetMemberUserName, CancellationToken cancellationToken)
    {
        FollowStatus followStatus = new();

        ObjectId? followedId = await _userRepository.GetObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

        if (followedId is null)
        {
            followStatus.IsTargetMemberNotFound = true;

            return followStatus;
        }

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);

        session.StartTransaction();

        try
        {
            DeleteResult deleteResult = await _collection.DeleteOneAsync<Follow>(doc =>
            doc.FollowerId == playerId &&
            doc.FollowedMemberId == followedId,
            cancellationToken);

            if (deleteResult.DeletedCount == 0)
            {
                followStatus.IsAlreadyUnfollowed = true;
                return followStatus;
            }

            #region UpdateCounters
            UpdateDefinition<RootModel> updateFollowingsCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowingsCount, -1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == playerId, updateFollowingsCount, null, cancellationToken);

            UpdateDefinition<RootModel> updateFollowersCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowersCount, -1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == followedId, updateFollowersCount, null, cancellationToken);
            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            followStatus.IsSuccess = true;
        }
        catch (System.Exception ex)
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
            _logger.LogInformation("MongoDB transaction/sesssion is finished");
        }

        return followStatus;
    }

    public async Task<bool> CheckIsFollowingAsync(ObjectId playerId, RootModel rootModel, CancellationToken cancellationToken) =>
    await _collection.Find<Follow>(
        follow => follow.FollowerId == playerId && follow.FollowedMemberId == rootModel.Id)
        .AnyAsync(cancellationToken);

    public async Task<PagedList<RootModel>> GetAllAsync(FollowParams followParams, CancellationToken cancellationToken)
    {
        PagedList<RootModel> rootModels;

        if (followParams.Predicate == FollowPredicateEnum.Followings)
        {
            IMongoQueryable<RootModel>? query = _collection.AsQueryable<Follow>()
                .Where(follow => follow.FollowerId == followParams.UserId)
                .Join(_collectionUsers.AsQueryable<RootModel>(),
                     follow => follow.FollowedMemberId,
                     rootModel => rootModel.Id,
                      (follow, appUser) => appUser);

             return rootModels = await PagedList<RootModel>
                .CreatePagedListAsync(query, followParams.PageNumber, followParams.PageSize, cancellationToken);
        }
        else if (followParams.Predicate == FollowPredicateEnum.Followers)
        {
            IMongoQueryable<RootModel>? query = _collection.AsQueryable<Follow>()
                .Where(follow => follow.FollowedMemberId == followParams.UserId)
                .Join(_collectionUsers.AsQueryable<RootModel>(),
                follow => follow.FollowerId,
                rootModel => rootModel.Id,
                (follow, rootModel) => rootModel);

            return rootModels = await PagedList<RootModel>
                .CreatePagedListAsync(query, followParams.PageNumber, followParams.PageSize, cancellationToken);
        }

        return [];
    }
}
