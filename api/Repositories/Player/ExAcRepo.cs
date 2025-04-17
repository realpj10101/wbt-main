using api.DTOs.Helpers;
using api.Extensions;
using api.Models.Helpers;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Player;

public class ExAcRepo : IExAcRepo
{
    private const string SessionExpiredMessage = "You session has expired. Login again.";

    #region Db and Token Settings

    private readonly IMongoCollection<AppUser>? _collection;
    private readonly IMongoCollection<RefreshToken> _collectionRefreshTokens;
    private readonly UserManager<AppUser> _userManager;
    private readonly IExampleTokenService _exampleTokenService;
    private readonly IPlayerUserRepository _playerUserRepository;

    public ExAcRepo(IMongoClient client, IMyMongoDbSettings dbSettings,
        UserManager<AppUser> userManager, IExampleTokenService exampleTokenService,
        IPlayerUserRepository playerUserRepository)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.CollectionUsers);
        _collectionRefreshTokens = database.GetCollection<RefreshToken>(AppVariablesExtensions.CollectionRefreshTokens);
        _userManager = userManager;
        _exampleTokenService = exampleTokenService;
        _playerUserRepository = playerUserRepository;
    }

    #endregion

    private async Task RevokedAllRefreshTokenAsync(ObjectId userId, CancellationToken cancellationToken)
    {
        UpdateDefinition<RefreshToken> updateDefinition = Builders<RefreshToken>.Update
            .Set(token => token.IsRevoked, true);

        await _collectionRefreshTokens.UpdateManyAsync(
            token => token.UserId == userId, updateDefinition, null, cancellationToken
        );
    }

    public async Task<OperationResult<LoginResult>> CreateAsync(RegisterDto registerDto,
        CancellationToken cancellationToken)
    {
        LoggedInDto loggedInDto = new();

        AppUser appUser = Mappers.ConvertRegisterDtoToAppUser(registerDto);

        IdentityResult? userCreatedResult = await _userManager.CreateAsync(appUser, registerDto.Password);

        if (userCreatedResult.Succeeded)
        {
            IdentityResult? roleResult = await _userManager.AddToRoleAsync(appUser, "player");

            if (!roleResult.Succeeded)
                return new OperationResult<LoginResult>(
                    IsSuccess: false,
                    Error: new CustomError(
                        Code: ErrorCode.NetIdentifyFailed,
                        Message: "Failed to create role"
                    )
                );

            var token = await _exampleTokenService.GenerateTokenAsync(appUser, cancellationToken);

            return new OperationResult<LoginResult>(
                IsSuccess: true,
                Result: new LoginResult(
                    Mappers.ConvertAppUserToExLoggedInDto(appUser),
                    token
                ),
                Error: null
            );
        }
        else
        {
            string? errorMessage = userCreatedResult.Errors.FirstOrDefault()?.Description;

            return new OperationResult<LoginResult>(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.NetIdentifyFailed,
                    Message: errorMessage
                )
            );
        }

        // return new OperationResult<LoginResult>(
        //     IsSuccess: false,
        //     Error: new CustomError(
        //         Code: ErrorCode.IsAccountCreationFailed,
        //         Message: "Account creation failed. Try again later."
        //     )
        // );
    }

    public async Task<OperationResult<LoginResult>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
    {
        LoggedInDto loggedInDto = new();

        AppUser? appUser;

        appUser = await _userManager.FindByEmailAsync(loginDto.Email);

        if (appUser is null)
        {
            return new OperationResult<LoginResult>(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.NetIdentifyFailed,
                    Message: "Credentials are invalid"
                )
            );
        }

        bool isPassCorrect = await _userManager.CheckPasswordAsync(appUser, loginDto.Password);

        if (!isPassCorrect)
        {
            return new OperationResult<LoginResult>(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.IsWrongCreds,
                    Message: "Invalid credentials"
                )
            );
        }

        TokenDto? token = await _exampleTokenService.GenerateTokenAsync(appUser, cancellationToken);

        return new OperationResult<LoginResult>(
            IsSuccess: true,
            new LoginResult(
                Mappers.ConvertAppUserToExLoggedInDto(appUser),
                token
            ),
            Error: null
        );
    }

    public async Task<OperationResult<TokenDto>> RefreshTokensAsync(
        RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
    {
        RefreshToken expectedTokenFromDb = await _collectionRefreshTokens.Find(
                                               token => token.JtiValue == refreshTokenRequest.JtiValue
                                           ).SingleOrDefaultAsync(cancellationToken) ??
                                           throw new ArgumentNullException(nameof(expectedTokenFromDb),
                                               "cannot be null");

        // if () //TODO logged in from new device/location.
        // {
        //     
        // }

        if (expectedTokenFromDb.UsedAt is not null)
        {
            await RevokedAllRefreshTokenAsync(expectedTokenFromDb.UserId, cancellationToken);
        }

        bool isInvalid = !TokenHasher.ValidateToken(
            refreshTokenRequest.TokenValueRaw, expectedTokenFromDb.TokenValueHashed
        );

        if (
            expectedTokenFromDb.ExpiresAt < DateTimeOffset.UtcNow
            || expectedTokenFromDb.IsRevoked
            || isInvalid
        )
        {
            return new OperationResult<TokenDto>(
                false,
                Error: new CustomError(
                    ErrorCode.IsSessionExpired,
                    SessionExpiredMessage
                )
            );
        }

        UpdateDefinition<RefreshToken>? revokedTokenUpdateDef = Builders<RefreshToken>.Update
            .Set(token => token.UsedAt, DateTimeOffset.UtcNow)
            .Set(t => t.IsRevoked, true);

        await _collectionRefreshTokens.UpdateOneAsync(
            token => token.Id == expectedTokenFromDb.Id, revokedTokenUpdateDef, null, cancellationToken
        );

        AppUser? appUser = await _playerUserRepository.GetByIdAsync(expectedTokenFromDb.UserId, cancellationToken);
        if (appUser is null)
        {
            return new OperationResult<TokenDto>(
                false,
                Error: new CustomError(
                    ErrorCode.IsSessionExpired,
                    SessionExpiredMessage
                )
            );
        }

        return new OperationResult<TokenDto>(
            true,
            await _exampleTokenService.GenerateTokensAsync(refreshTokenRequest, appUser, cancellationToken),
            null
        );
    }
}