using api.Enums;
using api.Extensions;
using api.Helpers;
using api.Models.Helpers;

namespace api.Repositories;

public class FollowCoachRepository : IFollowCoachRepository
{
    private readonly IMongoClient _client; // used for Session
    private readonly IMongoCollection<Follow> _collection;
    private readonly ITokenService _tokenService;
    private readonly IMongoCollection<RootModel> _collectionUsers;
    private readonly ICoachUserRepository _coachUserRepository;
    private readonly ILogger<FollowCoachRepository> _logger;

    public FollowCoachRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService, ICoachUserRepository coachUserRepository, ILogger<FollowCoachRepository> logger
    )
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Follow>(AppVariablesExtensions.collectionFollows);
        _collectionUsers = dbName.GetCollection<RootModel>(AppVariablesExtensions.collectionCoaches);

        _tokenService = tokenService;

        _coachUserRepository = coachUserRepository;

        _logger = logger;
    }

    public async Task<FollowStatus> CreateFollowCoAsync(ObjectId coachId, string targetMemberUserName, CancellationToken cancellationToken)
    {
        FollowStatus followCoStatus = new();

        ObjectId? followedId = await _coachUserRepository.GetCoObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

        if (followedId is null || !followedId.HasValue || followedId.Equals(ObjectId.Empty))
        {
            followCoStatus.IsTargetMemberNotFound = true;

            return followCoStatus;
        }

        if (coachId == followedId)
        {
            followCoStatus.IsFollowingThemself = true;

            return followCoStatus;
        }

        bool isfolowingAgain = await _collection.Find<Follow>(followDoc =>
            followDoc.FollowedMemberId == followedId).AnyAsync(cancellationToken);

        if (isfolowingAgain)
        {
            followCoStatus.IsAlreadyFollowed = true;

            return followCoStatus;
        }

        Follow follow = CoachMappers.ConvertFollowIdsToFollow(coachId, followedId.Value);

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);

        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, follow, null, cancellationToken);

            #region UpdateCounters
            UpdateDefinition<RootModel> updateFollowingsCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowingsCount, 1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == coachId, updateFollowingsCount, null, cancellationToken);

            UpdateDefinition<RootModel> updateFollowersCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowersCount, 1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == followedId, updateFollowersCount, null, cancellationToken);
            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            followCoStatus.IsSuccess = true;
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
            _logger.LogInformation("MongoDB transaction/session is finished");
        }

        return followCoStatus;
    }

    public async Task<FollowStatus> RemoveAsync(ObjectId coachId, string targetMemberUserName, CancellationToken cancellationToken)
    {
        FollowStatus followStatus = new();

        ObjectId? followedId = await _coachUserRepository.GetCoObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

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
            doc.FollowerId == coachId &&
            doc.FollowedMemberId == followedId,
            cancellationToken);

            if (deleteResult.DeletedCount == 0)
            {
                followStatus.IsAlreadyUnfollowed = true;
                return followStatus;
            }

            #region UpdateCounters
            UpdateDefinition<RootModel> updateFollowingsCoCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowingsCount, -1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == coachId, updateFollowingsCoCount, null, cancellationToken);

            UpdateDefinition<RootModel> updateFollowersCoCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowersCount, -1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == followedId, updateFollowersCoCount, null, cancellationToken);
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

    public async Task<bool> CheckIsFollowingAsync(ObjectId coachId, RootModel rootModel, CancellationToken cancellationToken) =>
         await _collection.Find<Follow>(
            follow => follow.FollowerId == coachId &&
            follow.FollowedMemberId == rootModel.Id)
            .AnyAsync(cancellationToken);

    public async Task<PagedList<RootModel>> GetAllAsync(FollowParams followParams, CancellationToken cancellationToken)
    {
        PagedList<RootModel>? rootModels;

        if (followParams.Predicate == FollowPredicateEnum.Followings)
        {
            IMongoQueryable<RootModel>? query = _collection.AsQueryable<Follow>()
                .Where(follow => follow.FollowerId == followParams.UserId)
                .Join(_collectionUsers.AsQueryable<RootModel>(),
                follow => follow.FollowedMemberId,
                rootModel => rootModel.Id,
                (follow, rootModel) => rootModel);

            return rootModels = await PagedList<RootModel>
                .CreatePagedListAsync(query, followParams.PageNumber, followParams.PageSize, cancellationToken);
        }
        else if (followParams.Predicate == FollowPredicateEnum.Followers) 
        {
            IMongoQueryable<RootModel>? query = _collection.AsQueryable<Follow>()
                .Where(follow => follow.FollowedMemberId == followParams.UserId)
                .Join(_collectionUsers.AsQueryable(),
                follow => follow.FollowerId,
                rootModel => rootModel.Id,
                (follow, rootModel) => rootModel);

            return rootModels = await PagedList<RootModel>
                .CreatePagedListAsync(query, followParams.PageNumber, followParams.PageSize, cancellationToken);
        }

        return [];
    }
}