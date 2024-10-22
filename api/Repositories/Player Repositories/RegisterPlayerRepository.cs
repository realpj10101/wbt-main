using api.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories;
public class RegisterPlayerRepository : IRegisterPlayerRepository
{
    private readonly IMongoCollection<RootModel>? _collection;
    private readonly UserManager<RootModel> _userManager;
    private readonly ITokenService _tokenService; // save user credential as a token

    public RegisterPlayerRepository(IMongoClient client, IMyMongoDbSettings dbSettings, UserManager<RootModel> userManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionPlayers);
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<LoggedInDto> CreateAsync(RegisterPlayerDto registerPlayerDto, CancellationToken cancellationToken)
    {
        LoggedInDto loggedInDto = new();

        RootModel player = Mappers.ConvertRegisterPlayerDtoToRootModel(registerPlayerDto);

        IdentityResult? playerCreatedResult = await _userManager.CreateAsync(player, registerPlayerDto.Password);

        if (playerCreatedResult.Succeeded)
        {
            IdentityResult? roleResult = await _userManager.AddToRoleAsync(player, "member");

            if (!roleResult.Succeeded)
                return loggedInDto;

            string? token = await _tokenService.CreateToken(player, cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                return Mappers.ConvertRootModelToLoggedInDto(player, token);
            }
        }
        else
        {
            foreach (IdentityError error in playerCreatedResult.Errors)
            {
                loggedInDto.Errors.Add(error.Description);
            }
        }

        return loggedInDto;
    }

    public async Task<LoggedInDto> LoginAsync(LoginDto playerInput, CancellationToken cancellationToken)
    {
        LoggedInDto loggedInDto = new();

        RootModel? player;

        player = await _userManager.FindByEmailAsync(playerInput.Email);

        if (player is null)
        {
            loggedInDto.IsWrongCreds = true;
            return loggedInDto;
        }

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(player, playerInput.Password);

        if (!isPasswordCorrect)
        {
            loggedInDto.IsWrongCreds = true;
            return loggedInDto;
        }

        string? token = await _tokenService.CreateToken(player, cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            return Mappers.ConvertRootModelToLoggedInDto(player, token);
        }

        return loggedInDto; 
    }

    

    public async Task<LoggedInDto?> ReloadLoggedInPlayerAsync(string hashedUserId, string token, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null)
            return null;

        RootModel player = await _collection.Find<RootModel>(player => player.Id == playerId).FirstOrDefaultAsync(cancellationToken);

        return player is null
            ? null
            : Mappers.ConvertRootModelToLoggedInDto(player, token);
    }

    public async Task<UpdateResult?> UpdateLastActive(string hashedUserId, CancellationToken cancellationToken)
    {
        ObjectId? playerId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (playerId is null) return null;

        UpdateDefinition<RootModel> newLastActive = Builders<RootModel>.Update
        .Set(player =>
            player.LastActive, DateTime.UtcNow);

        return await _collection.UpdateOneAsync<RootModel>(player =>
            player.Id == playerId, newLastActive, null, cancellationToken);
    }
}
