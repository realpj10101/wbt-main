using api.DTOs.Coach_DTOs;
using api.Extensions;
using api.Interfaces.Coach;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Coach;

public class RegisterCoachRepository : IRegisterCoachRepository
{
    #region Vars and Constructor

    private readonly IMongoCollection<AppUser?> _collection;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterCoachRepository(IMongoClient client, IMyMongoDbSettings dbSettings,
        UserManager<AppUser> userManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        _userManager = userManager;
        _tokenService = tokenService;
    }

    #endregion

    /// <summary>
    /// Create an AppUser and insert in db
    /// Check if the user doesn't already exist.
    /// </summary>
    /// <param name="userInput"></param>
    /// <param name="registerCoachDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>LoggedInDto</returns>
    public async Task<LoggedInCoachDto> RegisterCoachAsync(RegisterCoachDto registerCoachDto,
        CancellationToken cancellationToken)
    {
        LoggedInCoachDto loggedInCoachDto = new();

        AppUser appUser = CoachMappers.ConvertRegisterCoachDtoToAppUser(registerCoachDto);
        
        
    }
}