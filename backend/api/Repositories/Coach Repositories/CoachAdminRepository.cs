using Microsoft.AspNetCore.Identity;

namespace api.Repositories;

public class CoachAdminRepository : ICoachAdminRepository
{
      #region Db and Token Settings
    private readonly IMongoCollection<RootModel>? _collection;
    private readonly UserManager<RootModel> _coachManager;

    // constructor - dependency injection
    public CoachAdminRepository(IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<RootModel> coachManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>("coaches");

        _coachManager = coachManager;
    }
    #endregion

    public async Task<IEnumerable<CoachWithRoleDto>> GetCoachesWithRolesAsync()
    {
        List<CoachWithRoleDto> coachWithRoles = [];

        IEnumerable<RootModel> coaches = _coachManager.Users;

        foreach (RootModel coach in coaches)
        {
            IEnumerable<string> roles = await _coachManager.GetRolesAsync(coach);

            coachWithRoles.Add(
                new CoachWithRoleDto(
                    UserName: coach.UserName!,
                    Roles: roles
                )
            );
        }

        return coachWithRoles;
    }

    public async Task<bool> DeleteCoachAsync(string userName)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SuspendCoachAsync(string userName)
    {
        throw new NotImplementedException();
    }
}
