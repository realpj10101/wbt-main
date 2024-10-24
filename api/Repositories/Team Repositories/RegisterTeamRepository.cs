using api.DTOs.Team;
using api.Extensions;
using api.Interfaces.Team;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Template;

namespace api.Repositories.Team;

public class RegisterTeamRepository : IRegisterTeamRepository
{
    private readonly IMongoCollection<RootModel>? _collection;
    private readonly UserManager<RootModel> _userManager;
    private readonly ITokenService _tokenService;

    public RegisterTeamRepository(IMongoClient client, MongoDbSettings dbSettings, UserManager<RootModel> userManager, ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSettings.DatabaseName);
        _collection = database.GetCollection<RootModel>(AppVariablesExtensions.collectionTeams);
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<LoggedInTeamDto> CreateAsync(RegisterTeamDto registerTeamDto, CancellationToken cancellationToken)
    {
        LoggedInTeamDto loggedInTeamDto = new();

        RootModel team = TeamMappers.ConvertRegisterTeamDtoToRootModel(registerTeamDto);

        IdentityResult? teamCreatedResult = await _userManager.CreateAsync(team, registerTeamDto.Password);

        if (teamCreatedResult.Succeeded)
        {
            IdentityResult? roleResult = await _userManager.AddToRoleAsync(team, "team");

            if (!roleResult.Succeeded)
                return loggedInTeamDto;

            string? token = await _tokenService.CreateToken(team, cancellationToken);

            if (!string.IsNullOrEmpty(token))
            {
                return TeamMappers.ConvertRootModelToLoggedInTeamDto(team, token);
            }
        }
        else
        {
            foreach (IdentityError error in teamCreatedResult.Errors)
            {
                loggedInTeamDto.Errors.Add(error.Description);
            }
        }

        return loggedInTeamDto;
    }

    public async Task<LoggedInTeamDto> LoginAsync(LoginTeamDto teamInput, CancellationToken cancellationToken)
    {
        LoggedInTeamDto loggedInTeamDto = new();

        RootModel? team;

        team = await _userManager.FindByEmailAsync(teamInput.Email);

        if (team is null)
        {
            loggedInTeamDto.IsWrongCreds = true;
            return loggedInTeamDto;
        }

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(team, teamInput.Password);

        if (!isPasswordCorrect)
        {
            loggedInTeamDto.IsWrongCreds = true;
            return loggedInTeamDto;
        }

        string? token = await _tokenService.CreateToken(team, cancellationToken);

        if (!string.IsNullOrEmpty(token))
        {
            return TeamMappers.ConvertRootModelToLoggedInTeamDto(team, token);
        }

        return loggedInTeamDto;
    }

    public async Task<LoggedInTeamDto?> ReloadLoggedInTeamAsync(string hashedUserId, string token, CancellationToken cancellationToken)
    {
        ObjectId? teamId = await _tokenService.GetActualUserIdAsync(hashedUserId, cancellationToken);

        if (teamId is null)
            return null;

        RootModel team = await _collection.Find<RootModel>(team => team.Id ==  teamId).FirstOrDefaultAsync(cancellationToken);

        return team is null
            ? null
            : TeamMappers.ConvertRootModelToLoggedInTeamDto(team, token);
    }
}