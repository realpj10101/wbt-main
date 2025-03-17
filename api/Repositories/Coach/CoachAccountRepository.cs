using api.DTOs.Coach_DTOs;
using api.Extensions;
using api.Interfaces.Coach;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Coach;

public class CoachAccountRepository : ICoachAccountRepository
{
    #region Vars and Constructor

    private readonly IMongoCollection<AppUser?> _collection;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public CoachAccountRepository(IMongoClient client, IMyMongoDbSettings dbSettings,
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
    public async Task<LoggedInDto> RegisterCoachAsync(RegisterDto userInput,
        CancellationToken cancellationToken)
    {
        LoggedInDto loggedInDto = new();

        AppUser appUser = Mappers.ConvertRegisterDtoToAppUser(userInput);
        
        // AppUser appUser = CoachMappers.ConvertRegisterCoachDtoToAppUser(registerCoachDto);

        IdentityResult? coachCreatedResult = await _userManager.CreateAsync(appUser, userInput.Password);

        if (coachCreatedResult.Succeeded)
        {
            IdentityResult? roleResult = await _userManager.AddToRoleAsync(appUser, "coach");

            if (!roleResult.Succeeded)
                return loggedInDto;

            string? token = await _tokenService.CreateToken(appUser, cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                return Mappers.ConvertAppUserToLoggedInDto(appUser, token);
            }
        }
        else
        {
            foreach (IdentityError error in coachCreatedResult.Errors)
            {
                loggedInDto.Errors.Add(error.Description);
            }
        }

        return loggedInDto;
    }

    public async Task<LoggedInDto> LoginAsync(LoginDto userInput, CancellationToken cancellationToken)
    {
        LoggedInDto loggedInDto = new();

        AppUser? appUser;

        appUser = await _userManager.FindByEmailAsync(userInput.Email);

        if (appUser is null)
        {
            loggedInDto.IsWrongCreds = true;

            return loggedInDto;
        }
        
        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(appUser, userInput.Password);

        if (!isPasswordCorrect)
        {
            loggedInDto.IsWrongCreds = true;

            return loggedInDto;
        }

        string? token = await _tokenService.CreateToken(appUser, cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            return Mappers.ConvertAppUserToLoggedInDto(appUser, token);
        }

        return loggedInDto;
    }

    public async Task<LoggedInDto?> ReloadLoggedInCoachAsync(string hashedUserId, string token,
        CancellationToken cancellationToken)
    {
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (coachId is null) return null;
        
        AppUser appUser = await _collection.Find<AppUser>(appUser => appUser.Id == coachId).FirstOrDefaultAsync(cancellationToken);

        return appUser is null
            ? null
            : Mappers.ConvertAppUserToLoggedInDto(appUser, token);
    }

    public async Task<UpdateResult?> UpdateLastActive(string hashedUserId, CancellationToken cancellationToken)
    {
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (coachId is null) return null;

        UpdateDefinition<AppUser> newLastActive = Builders<AppUser>.Update
            .Set(appUser => appUser.LastActive, DateTime.UtcNow);

        return await _collection.UpdateOneAsync<AppUser>(coach =>
            coach.Id == coachId, newLastActive, null, cancellationToken);
    }
}