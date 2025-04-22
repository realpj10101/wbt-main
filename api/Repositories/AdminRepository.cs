using api.DTOs.Helpers;
using api.DTOs.Team_DTOs;
using api.Enums;
using api.Extensions;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Player;

public class AdminRepository : IAdminRepository
{
    #region DB and Token Settings

    private readonly IMongoCollection<AppUser>? _collection;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMongoCollection<Team> _collectionTeams;

    public AdminRepository(IMongoClient client, IMyMongoDbSettings dbSetting, UserManager<AppUser> userManager,
        ITokenService tokenService)
    {
        var database = client.GetDatabase(dbSetting.DatabaseName);
        _collection = database.GetCollection<AppUser>(AppVariablesExtensions.CollectionUsers);
        _collectionTeams = database.GetCollection<Team>(AppVariablesExtensions.CollectionTeams);

        _userManager = userManager;
    }
    
    #endregion

    #region CRUD

    public async Task<IEnumerable<PlayerWithRoleDto>> GetUsersWithRoleAsync()
    {
        List<PlayerWithRoleDto> playersWithRoleDto = [];

        IEnumerable<AppUser> appUsers = _userManager.Users;

        foreach (AppUser appUser in appUsers)
        {
            IEnumerable<string> roles = await _userManager.GetRolesAsync(appUser);

            playersWithRoleDto.Add(
                new PlayerWithRoleDto(
                    UserName: appUser.UserName!,
                    Roles: roles
                )
            );
        }

        return playersWithRoleDto;
    }

    public async Task<DeleteResult?> DeleteUserAsync(string targetUserName, CancellationToken cancellationToken)
    {
        ObjectId playerId = await _collection.AsQueryable()
            .Where(doc => doc.UserName == targetUserName)
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        AppUser? appUser = await GetByIdAsync(playerId, cancellationToken);

        if (appUser is null) return null;

        return await _collection.DeleteOneAsync<AppUser>(appUser => appUser.Id == playerId, null, cancellationToken);
    }

    public async Task<AppUser?> GetByIdAsync(ObjectId playerId, CancellationToken cancellationToken)
    {
        AppUser? appUser = await _collection.Find<AppUser>(doc =>
            doc.Id == playerId).SingleOrDefaultAsync(cancellationToken);

        if (appUser is null) return null;

        return appUser;
    }
    
    public async Task<OperationResult<ShowTeamDto>> UpdateVerifiedStatus(string teamName,
        CancellationToken cancellationToken)
    {
        ObjectId teamId = await _collectionTeams.AsQueryable()
            .Where(doc => doc.TeamName.ToUpper() == teamName.ToUpper())
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);
            
        if (teamId.Equals(null))
        {
            return new OperationResult<ShowTeamDto>(
                false,
                Error: new CustomError(
                    ErrorCode.TeamNotFound,
                    Message: "Team not found."
                )
            );
        }

        UpdateDefinition<Team> statusResult = Builders<Team>.Update
            .Set(s => s.Status, Status.Verified)
            .Set(s => s.RejectionReason, "");

        await _collectionTeams.UpdateOneAsync(t => t.Id == teamId, statusResult, null, cancellationToken);
        
        Team verifiedTeam = await _collectionTeams.Find(t => t.Id == teamId).FirstOrDefaultAsync(cancellationToken);

        return new OperationResult<ShowTeamDto>(
            true,
            Result: Mappers.ConvertTeamToShowTeamDto(verifiedTeam)
        );
    }

    public async Task<OperationResult<ShowTeamDto>> UpdateRejectStatus(string teamName, UpdateRejectStatus reason,
        CancellationToken cancellationToken)
    {
        ObjectId teamId = await _collectionTeams.AsQueryable()
            .Where(doc => doc.TeamName.ToUpper() == teamName.ToUpper())
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);
            
        if (teamId.Equals(null))
        {
            return new OperationResult<ShowTeamDto>(
                false,
                Error: new CustomError(
                    ErrorCode.TeamNotFound,
                    Message: "Team not found."
                )
            );
        }
        
        UpdateDefinition<Team> statusResult = Builders<Team>.Update
            .Set(s => s.Status, Status.Rejected)
            .Set(s => s.RejectionReason, reason.RejectReason);

        await _collectionTeams.UpdateOneAsync(t => t.Id == teamId, statusResult, null, cancellationToken);
        
        Team verifiedTeam = await _collectionTeams.Find(t => t.Id == teamId).FirstOrDefaultAsync(cancellationToken);

        return new OperationResult<ShowTeamDto>(
            true,
            Result: Mappers.ConvertTeamToShowTeamDto(verifiedTeam)
        );
    }

    #endregion
}