using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;

namespace api.Repositories.Player;

public class MemberRepository : IMemberRepository
{
    private readonly IMongoCollection<AppUser>? _collection;

    public MemberRepository(IMongoClient client, IMyMongoDbSettings dbSettings)
    {
        IMongoDatabase? database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
    }
    
    public async Task<PagedList<AppUser>> GetAllAsync(PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        IMongoQueryable<AppUser> query = _collection.AsQueryable();

        return await PagedList<AppUser>.CreatePagedListAsync(query, paginationParams.PageNumber,
            paginationParams.PageSize, cancellationToken);
    }

    public async Task<PlayerDto?> GetByIdAsync(string playerId, CancellationToken cancellationToken)
    {
        AppUser appUser = await _collection.Find(appUser =>
            appUser.Id.ToString() == playerId).FirstOrDefaultAsync(cancellationToken);

        if (appUser.Id.ToString() is not null) return Mappers.ConvertAppUserToPlayerDto(appUser);

        return null;
    }

    public async Task<PlayerDto?> GetByUserNameAsync(string playerUserName, CancellationToken cancellationToken)
    {
        AppUser appUser = await _collection.Find(appUser =>
            appUser.NormalizedUserName == playerUserName).FirstOrDefaultAsync(cancellationToken);

        if (appUser.Id.ToString() is not null) return Mappers.ConvertAppUserToPlayerDto(appUser);

        return null;
    }
}