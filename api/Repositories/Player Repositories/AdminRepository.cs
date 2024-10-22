using Microsoft.AspNetCore.Identity;

namespace api.Repositories;

public class AdminRepository : IAdminRepository
{   
     #region Db and Token Settings
    private readonly IMongoCollection<RootModel>? _collection;
    private readonly UserManager<RootModel> _userManager;

    // constructor - dependency injection
    public AdminRepository(IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<RootModel> userManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>("players");

        _userManager = userManager;
    }
    #endregion

    public async Task<IEnumerable<PlayerWithRoleDto>> GetPlayersWithRolesAsync()
    {
        List<PlayerWithRoleDto> playerWithRoles = [];

        IEnumerable<RootModel> players = _userManager.Users;

        foreach (RootModel player in players)
        {
            IEnumerable<string> roles = await _userManager.GetRolesAsync(player);

            playerWithRoles.Add(
                new PlayerWithRoleDto(
                    UserName: player.UserName!,
                    Roles: roles
                )
            );
        }

        return playerWithRoles;
    }

    public async Task<bool> DeletePlayerAsync(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SuspendPlayerAsync(string userName)
    {
        throw new NotImplementedException();
    }
}