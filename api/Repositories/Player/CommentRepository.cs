using api.Extensions;
using api.Models.Helpers;

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
        _collection = dbName.GetCollection<Comment>(AppVariablesExtensions.CollectionComments);
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
        
        Comment comment = Mappers.ConvertCommentIdsToComment(userId, targetId.Value, content);

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
}