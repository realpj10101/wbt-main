using api.DTOs.Team;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Team;
using api.Models.Helpers;
using DnsClient.Protocol;

namespace api.Repositories.Team; 

public class JoinRepository : IJoinRepository
{
    private readonly IMongoClient _client;
    private readonly IMongoCollection<Join> _collection;
    private readonly ITokenService _tokenService;
    private readonly IMongoCollection<RootModel> _collectionUsers;
    private readonly ITeamUserRepository _teamUserRepository;
    private readonly ILogger<JoinRepository> _logger;

    public JoinRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings, ITokenService tokenService, ITeamUserRepository teamUserRepository, ILogger<JoinRepository> logger
    )
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Join>(AppVariablesExtensions.collectionJoines);

        _tokenService = tokenService;
        _collectionUsers = dbName.GetCollection<RootModel>
        (AppVariablesExtensions.collectionTeams);

        _teamUserRepository = teamUserRepository;

        _logger = logger;
    }

    public async Task<JoinStatus> CreateJoinAsync(ObjectId playerId, string targetTeamUsereName, CancellationToken cancellationToken)
    {
        JoinStatus joinStatus = new();

        ObjectId? joinedId = await _teamUserRepository.GetObjectIdByTeamUserNameAsync(targetTeamUsereName, cancellationToken);

        if (joinedId is null)
        {
            joinStatus.IsTargetTeamNotFound = true;

            return joinStatus;
        }

        if (playerId == joinedId)
        {
            joinStatus.IsJoiningThemself = true;

            return joinStatus;
        }

        bool IsJoiningAgain = await _collection.Find(joinDoc => joinDoc.JoinedTeamId == joinedId).AnyAsync(cancellationToken);

        if (IsJoiningAgain)
        {
            joinStatus.IsAlreadyJoined = true;

            return joinStatus;
        }

        Join join = TeamMappers.ConvertJoinsIdsToJoin(playerId, joinedId.Value);

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);

        session.StartTransaction();

        try
        {
            await _collection.InsertOneAsync(session, join, null, cancellationToken);

            #region UpdateCounters
            UpdateDefinition<RootModel> updateJoinersCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.JoinersCount, 1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                    rootModel.Id == joinedId, updateJoinersCount, null, cancellationToken);
            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            joinStatus.IsSuccess = true;
        }
        catch (System.Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "Join failed."
                + "MESSAGE" + ex.Message
                + "TRACE" + ex.StackTrace
            );
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session is finished");
        }

        return joinStatus;
    }

    public async Task<JoinStatus> RemoveAsync(ObjectId playerId, string targetTeamUsereName, CancellationToken cancellationToken)
    {
        JoinStatus joinStatus = new();

        ObjectId? joinedId = await _teamUserRepository.GetObjectIdByTeamUserNameAsync(targetTeamUsereName, cancellationToken);

        if (joinedId is null)
        {
            joinStatus.IsTargetTeamNotFound = true;

            return joinStatus;
        }

        using IClientSessionHandle session = await _client.StartSessionAsync(null, cancellationToken);

        session.StartTransaction();

        try
        {
            DeleteResult deleteResult = await _collection.DeleteOneAsync<Join>(doc =>
            doc.JoinerId == playerId &&
            doc.JoinedTeamId == joinedId,
            cancellationToken);

            if (deleteResult.DeletedCount == 0)
            {
                joinStatus.IsAlreadyLeft = true;

                return joinStatus;
            }

            #region  UpdateCounters
            UpdateDefinition<RootModel> updateJoinersCount = Builders<RootModel>.Update
                .Inc(rootModel => rootModel.JoinersCount, -1);

            await _collectionUsers.UpdateOneAsync<RootModel>(session, rootModel =>
                    rootModel.Id == joinedId, updateJoinersCount, null, cancellationToken);
            #endregion

            await session.CommitTransactionAsync(cancellationToken);

            joinStatus.IsSuccess = true;
        }
        catch (System.Exception ex)
        {
            await session.AbortTransactionAsync(cancellationToken);

            _logger.LogError(
                "Join failed"
                + "MESSAGE" + ex.Message
                + "TRACE" + ex.StackTrace
            );
        }
        finally
        {
            _logger.LogInformation("MongoDB transaction/session is finished");
        }

        return joinStatus;
    }

    public async Task<bool> CheckIsJoiningAsync(ObjectId playerId, RootModel rootModel, CancellationToken cancellationToken) =>
    await _collection.Find<Join>(
        join => join.JoinerId == playerId && join.JoinedTeamId == rootModel.Id)
        .AnyAsync(cancellationToken);

    // public async Task<bool> GetAllAsync(JoinParams joinParams, CancellationToken cancellationToken)
    // {
    //     PagedList<RootModel>? rootModels;

    //     if (joinParams.Predicate == "Members")
    //     {
    //         IMongoQueryable<RootModel>? query = _collection.AsQueryable<Join>()
    //             .Where(join => join.JoinerId == joinParams.UserId)
    //             .Join(_collectionUsers.AsQueryable<RootModel>(),
    //             join => join.JoinedTeamId,
    //             rootModel => rootModel.Id,
    //             (join, rootModel) => rootModel);

    //         rootModels = await PagedList<RootModel>
    //             .CreatePagedListAsync(query, joinParams.PageNumber, joinParams.PageSize, cancellationToken);
    //     }

    //     return rootModels
    // }
}