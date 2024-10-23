using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using api.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace api.Repositories;

public class RegisterCoachRepository : IRegisterCoachRepository
{
    private readonly IMongoCollection<RootModel>? _collection;
    private readonly UserManager<RootModel> _coachManager;
    private readonly ITokenService _tokenService; // save user credential as a token

    public RegisterCoachRepository(IMongoClient client, MongoDbSettings dbSettings, UserManager<RootModel> coachManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionCoaches);
        _coachManager = coachManager;
        _tokenService = tokenService;
    }

    public async Task<LoggedInCoachDto> CreateCoachAsync(RegisterCoachDto registerCoachDto, CancellationToken cancellationToken)
    {
        LoggedInCoachDto loggedInCoachDto = new();

        RootModel coach = CoachMappers.ConvertRegisterCoDtoToRootModel(registerCoachDto);

        IdentityResult? coachCreatedResult = await _coachManager.CreateAsync(coach, registerCoachDto.Password);

        if (coachCreatedResult.Succeeded)
        {
            IdentityResult? roleResult = await _coachManager.AddToRoleAsync(coach, "member");

            if (!roleResult.Succeeded)
                return loggedInCoachDto;

            string? token = await _tokenService.CreateToken(coach, cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                return CoachMappers.ConvertRootModelToLoggedInCoDto(coach, token);
            }
        }

        else
        {
            foreach (IdentityError error in coachCreatedResult.Errors)
            {
                loggedInCoachDto.Errors.Add(error.Description);
            }
        }

        return loggedInCoachDto;
    }

    public async Task<LoggedInCoachDto> LoginCoachAsync(LoginCoachDto coachInput, CancellationToken cancellationToken)
    {
        LoggedInCoachDto loggedInCoachDto = new();

        RootModel? coach;

        coach = await _coachManager.FindByEmailAsync(coachInput.Email);

        if (coach is null)
        {
            loggedInCoachDto.IsWrongCreds = true;
            return loggedInCoachDto;
        }

        if (!await _coachManager.CheckPasswordAsync(coach, coachInput.Password))
        {
            loggedInCoachDto.IsWrongCreds = true;
            return loggedInCoachDto;
        }

        string? token = await _tokenService.CreateToken(coach, cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            return CoachMappers.ConvertRootModelToLoggedInCoDto(coach, token);
        }

        return loggedInCoachDto;
    }

    public async Task<LoggedInCoachDto?> ReloadLoggedInCoachAsync(string hashedUserId, string token, CancellationToken cancellationToken)
    {
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (coachId is null)
            return null;

        RootModel rootModel = await _collection.Find<RootModel>(rootModel => rootModel.Id == coachId).FirstOrDefaultAsync(cancellationToken);

        return rootModel is null
            ? null
            : CoachMappers.ConvertRootModelToLoggedInCoDto(rootModel, token);
    }

    public async Task<UpdateResult?> UpdateCoLastActive(string hashedUserId, CancellationToken cancellationToken)
    {   
        ObjectId? coachId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (coachId is null) return null;

        UpdateDefinition<RootModel> newLastActive = Builders<RootModel>.Update
            .Set(rootModel =>
                rootModel.LastActive, DateTime.UtcNow);

        return await _collection.UpdateOneAsync<RootModel>(coach =>
            coach.Id == coachId, newLastActive, null, cancellationToken);
    }
}