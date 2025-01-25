using api.Extensions;
using api.Helpers;
using api.Interfaces.Player;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Player;

public class MemberRepository : IMemberRepository
{
    private readonly IMongoCollection<AppUser>? _collection;
    private readonly ITokenService _tokenService;
    private readonly IFollowRepository _followRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
        
    public MemberRepository(IMongoClient client, IMyMongoDbSettings dbSettings,
        ITokenService tokenService, IFollowRepository followRepository, UserManager<AppUser> userManager)
    {
        IMongoDatabase? database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        
        _tokenService = tokenService;
        _followRepository = followRepository;
        _userManager = userManager;
    }
    
    public async Task<PagedList<AppUser>> GetAllAsync(PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        IMongoQueryable<AppUser> query = _collection.AsQueryable();

        // AppUser appUser = ;

        // IdentityResult? roleResult = await _userManager.GetRolesAsync(); 
            
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

        if (appUser is null) return null;
        
        bool isFollowing = await _followRepository.CheckIsFollowingAsync(playerId.Value, appUser, cancellationToken);

        return appUser is not null
            ? Mappers.ConvertAppUserToPlayerDto(appUser, isFollowing)
            : null;
    }

    // public async Task<ObjectId?> GetByRoleIdAsync(string roleName,
    //     CancellationToken cancellationToken)
    // {
    //     
    // }
    
    // test
}