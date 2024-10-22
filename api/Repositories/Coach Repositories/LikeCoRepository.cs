using api.Extensions;
using api.Models.Helpers;

namespace api.Repositories;

public class LikeCoRepository : ILikeCoachRepository
{
    private readonly IMongoClient _client;
    private readonly IMongoCollection<Like> _collection;
    private readonly ITokenService _tokenService;
    private readonly IMongoCollection<RootModel> _collectionUsers;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LikeCoRepository> _logger;

    public LikeCoRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService, IUserRepository userRepository, ILogger<LikeCoRepository> logger
    )
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Like>(AppVariablesExtensions.collectionLikes);

        _tokenService = tokenService;
        _collectionUsers = dbName.GetCollection<RootModel>
        (AppVariablesExtensions.collectionCoaches);

        _userRepository = userRepository;

        _logger = logger;
    }

    public async Task<LikeStatus> CreateLikeCoAsync(ObjectId coachId, string targetMemberUserName, CancellationToken cancellationToken)
    {
        LikeStatus likeStatus = new();

        ObjectId? likedId = await _userRepository.GetObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

        if (likedId == null)
        {
            likeStatus.IsTargetMemberNotFound = true;

            return likeStatus;
        }

        if (coachId == likedId)
        {
            likeStatus.IsLikingThemself = true;

            return likeStatus;
        }

        bool IsLikingAgain = await _collection.Find(liekDoc => liekDoc.LikedMemberId == likedId).AnyAsync(cancellationToken);

        if (IsLikingAgain)
        {
            likeStatus.IsAlreadyLiked = true;

            return likeStatus;
        }

        Like like = CoachMappers.ConvertLikeIdsToLike(coachId, likedId.Value);

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);

        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, like, null, cancellationToken);

            #region UpdateCounters
            UpdateDefinition<RootModel> updateCoLikingsCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.LikingsCount, 1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == coachId, updateCoLikingsCount, null, cancellationToken);

            UpdateDefinition<RootModel> updateCoLikersCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.LikersCount, 1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == likedId, updateCoLikersCount, null, cancellationToken);
            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            likeStatus.IsSuccess = true;
        }
        catch (System.Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "Like failed"
                + "MESSAGE:" + ex.Message
                + "TRACE:" + ex.StackTrace
            );
        }
        finally 
        {
            _logger.LogInformation("MongoDb transaction/session is finished.");
        }

        return likeStatus;
    }

    public async Task<LikeStatus> RemoveLikeCoAsync(ObjectId coacId, string targetMemberUserName, CancellationToken cancellationToken)
    {
        LikeStatus likeStatus = new();

        ObjectId? likedId = await _userRepository.GetObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

        if (likedId is null)
        {
            likeStatus.IsTargetMemberNotFound = true;

            return likeStatus;
        }

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);

        session.StartTransaction();

        try
        {
            DeleteResult deleteResult = await _collection.DeleteOneAsync<Like>(doc => 
            doc.LikerId == coacId &&
            doc.LikedMemberId == likedId,
            cancellationToken);

            if (deleteResult.DeletedCount == 0)
            {
                likeStatus.IsAlreadyDisLiked = true;
                return likeStatus;
            }

            #region UpdateCounters
            UpdateDefinition<RootModel> updateLikingsCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowingsCount, -1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == coacId, updateLikingsCount, null, cancellationToken);

            UpdateDefinition<RootModel> updateLikersCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.FollowersCount, -1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                rootModel.Id == likedId, updateLikersCount, null, cancellationToken);
            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            likeStatus.IsSuccess = true;
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
        
        return likeStatus;
    }

    public async Task<bool> CheckIsLikingCoAsync(ObjectId coachId, RootModel rootModel, CancellationToken cancellationToken) =>
        await _collection.Find<Like>(
           like => like.LikerId == coachId && like.LikedMemberId == rootModel.Id)
           .AnyAsync(cancellationToken);
}