using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using Microsoft.AspNetCore.Http.HttpResults;

namespace api.Repositories.Player;

public class MemberRepository : IMemberRepository
{
    private readonly IMongoCollection<AppUser>? _collection;
    private readonly ITokenService _tokenService;
    private readonly IFollowRepository _followRepository;

    public MemberRepository(IMongoClient client, IMyMongoDbSettings dbSettings,
        ITokenService tokenService, IFollowRepository followRepository)
    {
        IMongoDatabase? database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        
        _tokenService = tokenService;
        _followRepository = followRepository;
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
        
        if (appUser is null) return null;

        if (appUser.Id.ToString() is not null) return Mappers.ConvertAppUserToPlayerDto(appUser);

        return null;
    }

    public async Task<PlayerDto?> GetByUserNameAsync(string playerUserName, string userIdHashed, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(userIdHashed, cancellationToken);
        
        if (playerId is null) return null;
        
        AppUser appUser = await _collection.Find(appUser =>
            appUser.NormalizedUserName == playerUserName.ToUpper()).FirstOrDefaultAsync(cancellationToken);

        bool isFollowing = await _followRepository.CheckIsFollowingAsync(playerId.Value, appUser, cancellationToken);

        return appUser is not null
            ? Mappers.ConvertAppUserToPlayerDto(appUser, isFollowing)
            : null;
    }
}