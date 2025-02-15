using api.Enums;
using api.Extensions;
using api.Helpers;
using api.Models.Helpers;
using Snappier;

namespace api.Repositories.Player;

public class CommentRepository : ICommentRepository
{
    private readonly IMongoClient _client;
    private readonly IMongoCollection<Comment> _collection;
    private readonly ITokenService _tokenService;
    private readonly IMongoCollection<AppUser> _collectionUsers;
    private readonly IPlayerUserRepository _playerUserRepository;
    private readonly ILogger<CommentRepository> _logger;

    public CommentRepository(
       IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService,
       IPlayerUserRepository playerUserRepository, ILogger<CommentRepository> logger)
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Comment>(AppVariablesExtensions.collectionComments);
        _collectionUsers = dbName.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        
        _tokenService = tokenService;
        _playerUserRepository = playerUserRepository;
        _logger = logger;
    }

    /// <summary>
    /// LoggedInUser Commented to members
    /// </summary>
    public async Task<CommentStatus> CreateAsync(
        ObjectId userId,
        string targetMemberUserName,
        string content,
        CancellationToken cancellationToken)
    {
        CommentStatus cS = new();

        ObjectId? targetId =
            await _playerUserRepository.GetObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);

        if (targetId is null)
        {
            cS.IsTargetMemberNotFound = true;

            return cS;
        }
        
        string? commenterName = await _collectionUsers.AsQueryable()
            .Where(doc => doc.Id == userId)
            .Select(doc => doc.UserName)
            .FirstOrDefaultAsync(cancellationToken);
        
        string? commentedMemberName = await _collectionUsers.AsQueryable()
            .Where(doc => doc.Id == targetId)
            .Select(doc => doc.UserName)
            .FirstOrDefaultAsync(cancellationToken);
        
        Comment comment = Mappers.ConvertCommentIdsToComment(userId, targetId.Value, commenterName, commentedMemberName, content);

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);
        
        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, comment, null, cancellationToken);

            #region UpdateCounters

            UpdateDefinition<AppUser> updateCommentingCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.CommentingCount, 1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == userId, updateCommentingCount, null, cancellationToken);

            UpdateDefinition<AppUser> updateCommentersCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.CommentersCount, 1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == targetId, updateCommentersCount, null, cancellationToken);

            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            cS.IsSuccess = true;
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "Comment failed."
                + "MESSAGE" + ex.Message
                + "TRACE" + ex.StackTrace
            );
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session finished.");
        }

        return cS; 
    }
    
    // Find comment by doc Id and delete it.
    public async Task<CommentStatus> DeleteAsync(ObjectId userId, string targetMemberUserName,
        CancellationToken cancellationToken)
    {
        CommentStatus cS = new();
        
        ObjectId? targetId = 
            await _playerUserRepository.GetObjectIdByUserNameAsync(targetMemberUserName, cancellationToken);
    
        if (targetId is null)
        {
            cS.IsTargetMemberNotFound = true;

            return cS;
        }

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);
        
        session.StartTransaction();

        try
        {
            DeleteResult deleteResult = await _collection.DeleteOneAsync(
                doc => doc.CommenterId == userId
                       && doc.CommentedMemberId == targetId, cancellationToken);

            if (deleteResult.DeletedCount < 1)
            {
                cS.IsAlreadyDeleted = true;

                return cS;
            }

            #region UpdateCounters

            UpdateDefinition<AppUser> updateCommentingCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.CommentingCount, -1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == userId, updateCommentingCount, null, cancellationToken);

            UpdateDefinition<AppUser> updateCommentersCount = Builders<AppUser>.Update
                .Inc(appUser => appUser.CommentersCount, -1);

            await _collectionUsers.UpdateOneAsync<AppUser>(session, appUser =>
                appUser.Id == targetId, updateCommentersCount, null, cancellationToken);

            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            cS.IsSuccess = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Delete Comment failed."
                + "MESSAGE" + ex.Message
                + "TRACE" + ex.StackTrace
            );
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session finished.");
        }

        return cS;
    }
    
    // get all commens/commentigs with pagination
    public async Task<PagedList<AppUser>> GetAllAsync(CommentParams commentParams, CancellationToken cancellationToken)
    {
        if (commentParams.Predicate == CommentPredicateEnum.Commentings) // Logged in user comments for other users
        {
            IMongoQueryable<AppUser> query = _collection.AsQueryable<Comment>()
                .Where(comment => comment.CommenterId == commentParams.UserId)
                .Join(_collectionUsers.AsQueryable(),
                    comment => comment.CommentedMemberId,
                    appUser => appUser.Id,
                    (comment, appUser) => appUser);
            
            return await PagedList<AppUser>
                .CreatePagedListAsync(query, commentParams.PageNumber, commentParams.PageSize, cancellationToken);
        }
        else if (commentParams.Predicate == CommentPredicateEnum.Commenters) // Users comments for logged in user
        {
            IMongoQueryable<AppUser> query = _collection.AsQueryable<Comment>()
                .Where(comment => comment.CommentedMemberId == commentParams.UserId)
                .Join(_collectionUsers.AsQueryable(),
                    comment => comment.CommenterId,
                    appUser => appUser.Id,
                    (comment, appUser) => appUser);
            
            return await PagedList<AppUser>
                .CreatePagedListAsync(query, commentParams.PageNumber, commentParams.PageSize, cancellationToken);
        }

        return [];
    }
}