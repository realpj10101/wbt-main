using api.Extensions;
using api.Interfaces.Player;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Player;

public class RegisterPlayerRepository : IRegisterPlayerRepository
{
    #region Vars and Constructor

    private readonly IMongoCollection<AppUser>? _collection;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterPlayerRepository(IMongoClient client, IMyMongoDbSettings dbSettings,
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
    /// <param name="registerDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>LoggedInDto</returns>
    public async Task<LoggedInDto> RegisterPlayerAsync(RegisterPlayerDto registerDto, CancellationToken cancellationToken)
    {
        LoggedInDto loggedInDto = new();

        AppUser appUser = Mappers.ConvertRegisterPlayerDtoToAppUser(registerDto);

        IdentityResult? userCreatedResult = await _userManager.CreateAsync(appUser, registerDto.Password);

        if (userCreatedResult.Succeeded)
        {
            IdentityResult? roleResult = await _userManager.AddToRoleAsync(appUser, "member");

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
            foreach (IdentityError error in userCreatedResult.Errors)
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

        bool isPassCorrect = await _userManager.CheckPasswordAsync(appUser, userInput.Password);

        if (!isPassCorrect)
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

    public async Task<LoggedInDto?> ReloadLoggedInUserAsync(string hashedUserId, string token,
        CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (userId is null)
            return null;

        AppUser appUser = await _collection.Find<AppUser>(appUser => appUser.Id == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return appUser is null
            ? null
            : Mappers.ConvertAppUserToLoggedInDto(appUser, token);
    }

    public async Task<UpdateResult?> UpdateLastActive(string hashedUserId, CancellationToken cancellationToken)
    {
        ObjectId? userId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (userId is null) return null;

        UpdateDefinition<AppUser> newLastActive = Builders<AppUser>.Update
            .Set(appUser =>
                appUser.LastActive, DateTime.UtcNow);

        return await _collection.UpdateOneAsync<AppUser>(user =>
            user.Id == userId, newLastActive, null, cancellationToken);
    }
}