using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Player;

public class AdminRepository : IAdminRepository
{
    #region DB and Token Settings

    private readonly IMongoCollection<AppUser>? _collection;
    private readonly UserManager<AppUser> _userManager;

    public AdminRepository(IMongoClient client, IMyMongoDbSettings dbSetting, UserManager<AppUser> userManager,
        ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSetting.DatabaseName);
        _collection = database.GetCollection<AppUser>("users");
            
        _userManager = userManager;    
    }
    #endregion

    #region CRUD

    public async Task<IEnumerable<PlayerWithRoleDto>> GetUsersWithRoleAsync()
    {
        List<PlayerWithRoleDto> playersWithRoleDto = [];

        IEnumerable<AppUser> appUsers = _userManager.Users;

        foreach (AppUser appUser in appUsers)
        {
            IEnumerable<string> roles = await _userManager.GetRolesAsync(appUser);
            
            playersWithRoleDto.Add(
                new PlayerWithRoleDto(
                    UserName: appUser.UserName!,
                    Roles: roles
                    )
                );
        }

        return playersWithRoleDto;
    }

    public async Task<DeleteResult?> DeleteUserAsync(string targetUserName, CancellationToken cancellationToken)
    {
        ObjectId playerId = await _collection.AsQueryable()
            .Where(doc => doc.UserName == targetUserName)
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        AppUser? appUser = await GetByIdAsync(playerId, cancellationToken);

        if (appUser is null) return null;

        return await _collection.DeleteOneAsync<AppUser>(appUser => appUser.Id == playerId, null, cancellationToken);
    }

    public async Task<AppUser?> GetByIdAsync(ObjectId playerId, CancellationToken cancellationToken)
    {
        AppUser? appUser = await _collection.Find<AppUser>(doc =>
            doc.Id == playerId).SingleOrDefaultAsync(cancellationToken);

        if (appUser is null) return null;

        return appUser;
    }
    #endregion
}