using api.Extensions;
using api.Helpers;
using AspNetCore.Identity.MongoDbCore.Infrastructure;

namespace api.Repositories;
public class PlayerRepository : IPlayerRepository
{
    #region db and token

    readonly IMongoCollection<RootModel>? _collection;

    public PlayerRepository(IMongoClient client, MongoDbSettings dbSettings)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionPlayers    );
        // _tokenService = tokenService;
    }

    #endregion

    public async Task<PagedList<RootModel>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        IMongoQueryable<RootModel> query = _collection.AsQueryable();

        return await PagedList<RootModel>.CreatePagedListAsync(query, paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);
    }

    public async Task<PlayerDto?> GetByIdAsync(string playerId, CancellationToken cancellationToken)
    {
        RootModel player = await _collection.Find<RootModel>(player =>
                player.Id.ToString() == playerId).FirstOrDefaultAsync(cancellationToken);

        if (player.Id.ToString() is not null)
        {
            return Mappers.ConvertRootModelToPlayerDto(player);
        }

        return null;
    }

    public async Task<PlayerDto?> GetByUserNameAsync(string playerUserName, CancellationToken cancellationToken)
    {
        RootModel player = await _collection.Find<RootModel>(player =>
                player.NormalizedUserName == playerUserName).FirstOrDefaultAsync(cancellationToken);

        if (player.Id.ToString() is not null)
        {
            return Mappers.ConvertRootModelToPlayerDto(player);
        }

        return null;
    }


}