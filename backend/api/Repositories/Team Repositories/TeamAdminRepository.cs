using api.DTOs.Team;
using api.Extensions;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories;

public class TeamAdminRepository : ITeamAdminRepository
{
    #region Db and Token Settings
    private readonly IMongoCollection<RootModel>? _collection;

    private readonly UserManager<RootModel> _userManager;

    public TeamAdminRepository(IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<RootModel> userManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionTeams);

        _userManager = userManager;
    }
    #endregion

    public async Task<IEnumerable<PlayerWithRoleDto>> GetPlayersWithRolesAsync()
    {
        List<PlayerWithRoleDto> playerWithRole = [];

        IEnumerable<RootModel> players = _userManager.Users;

        foreach (RootModel player in players)
        {
            IEnumerable<string> roles = await _userManager.GetRolesAsync(player);

            playerWithRole.Add(
                new PlayerWithRoleDto(
                    UserName: player.UserName!,
                    Roles: roles
                )
            );
        }

        return playerWithRole;
    }

    public async Task<bool> RemovePlayerAsync(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SuspendPlayerAsync(string userName)
    {
        throw new NotImplementedException();
    }
}