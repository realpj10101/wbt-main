using System.Runtime.InteropServices.JavaScript;
using api.DTOs.Helpers;
using api.DTOs.Team_DTOs;
using api.Enums;
using api.Extensions;
using api.Helpers;
using api.Interfaces.Teams;
using api.Models.Helpers;
using Microsoft.AspNetCore.Identity;

namespace api.Repositories.Player;

public class TeamRepository : ITeamRepository
{
    #region MyRegion

    private readonly IMongoClient _client;
    private readonly IMongoCollection<Team> _collection;
    private readonly IMongoCollection<AppUser> _collectionAppUser;
    private readonly ITokenService _tokenService;
    private readonly ILogger<TeamRepository> _logger;
    private readonly IPlayerUserRepository _playerUserRepository;
    private readonly UserManager<AppUser> _userManager;

    public TeamRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings,
        ITokenService tokenService, ILogger<TeamRepository> logger, IPlayerUserRepository playerUserRepository,
        UserManager<AppUser> userManager)
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Team>(AppVariablesExtensions.CollectionTeams);
        _collectionAppUser = dbName.GetCollection<AppUser>(AppVariablesExtensions.CollectionUsers);

        _tokenService = tokenService;

        _playerUserRepository = playerUserRepository;

        _logger = logger;
        _userManager = userManager;
    }

    #endregion

    // Create team and add member in
    public async Task<ShowTeamDto?> CreateAsync(
        ObjectId userId,
        CreateTeamDto userInput,
        CancellationToken cancellationToken)
    {
        Team userTeam = await _collection.Find(t => t.TeamName == userInput.TeamName)
            .FirstOrDefaultAsync(cancellationToken);

        if (userTeam is not null) return null;

        Team? team = Mappers.ConvertCreateTeamDtoToTeam(userId, userInput);

        await _collection.InsertOneAsync(team, null, cancellationToken);

        if (team is not null)
        {
            ShowTeamDto showTeamDto = Mappers.ConvertTeamToShowTeamDto(team);

            return showTeamDto;
        }

        return null;
    }

    // public async Task<TeamStatus> UpdateTeamAsync(
    //     ObjectId userId,
    //     UpdateTeamDto userInput, string targetTeamName, CancellationToken cancellationToken)
    // {
    //     TeamStatus tS = new();
    //
    //     ObjectId teamId = await _collection.AsQueryable()
    //         .Where(doc => doc.TeamName == targetTeamName)
    //         .Select(doc => doc.Id)
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     Team teamName = await _collection.Find(t => t.TeamName == targetTeamName)
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     if (teamName is null)
    //     {
    //         tS.IsTargetTeamNotFound = true;
    //         return tS;
    //     }
    //
    //     ObjectId memberId = await _collectionAppUser.AsQueryable()
    //         .Where(doc => doc.NormalizedUserName == userInput.UserName.ToUpper())
    //         .Select(doc => doc.Id)
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     if (userId == memberId)
    //     {
    //         tS.IsJoiningThemself = true;
    //         return tS;
    //     }
    //
    //     AppUser user = await _collectionAppUser.Find(u => u.Id == memberId).FirstOrDefaultAsync(cancellationToken);
    //
    //     if (user is null)
    //     {
    //         tS.IsTargetMemberNotFound = true;
    //         return tS;
    //     }
    //
    //     UpdateDefinition<Team> updatedTeam = Builders<Team>.Update
    //         .Set(t => t.TeamName, userInput.TeamName?.ToLower().Trim())
    //         .Set(t => t.TeamLevel, userInput.TeamLevel?.ToLower().Trim())
    //         .Set(t => t.Achievements, userInput.Achievements?.ToLower().Trim())
    //         .Set(t => t.GamesPlayed, userInput.GamesPlayed)
    //         .Set(t => t.GamesWon, userInput.GamesWon)
    //         .Set(t => t.GamesLost, userInput.GamesLost);
    //
    //     await _collection.UpdateOneAsync(
    //         doc => doc.Id == teamId, updatedTeam, null, cancellationToken
    //     );
    //
    //     Team team1 = await _collection.Find(t => t.MembersUserNames.Contains(userInput.UserName))
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     if (team1 == null)
    //     {
    //         tS.IsTargetTeamNotFound = true;
    //         return tS;
    //     }
    //
    //     EnrolledTeam? teamMap = Mappers.ConvertTeamToEnrolledTeamDto(team1);
    //
    //     UpdateDefinition<AppUser> updatedUser = Builders<AppUser>.Update
    //         .AddToSet(appUser => appUser.EnrolledTeams, teamMap);
    //
    //     await _collectionAppUser.UpdateOneAsync<AppUser>(appUser =>
    //         appUser.Id == memberId, updatedUser, null, cancellationToken);
    //
    //     tS.IsSuccess = true;
    //
    //     return tS;
    // }

    public async Task<PagedList<Team>?> GetAllAsync(PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        IMongoQueryable<Team> query = _collection.AsQueryable();

        PagedList<Team> teams = await PagedList<Team>.CreatePagedListAsync(
            query, paginationParams.PageNumber, paginationParams.PageSize, cancellationToken);

        return teams;
    }

    public async Task<ShowTeamDto?> GetByTeamNameAsync(string teamName, CancellationToken cancellationToken)
    {
        Team team = await _collection.Find(t =>
            t.TeamName == teamName.ToLower()).FirstOrDefaultAsync(cancellationToken);

        if (team is null) return null;

        if (team.TeamName is not null)
            return Mappers.ConvertTeamToShowTeamDto(team);

        return null;
    }

    public async Task<List<AppUser>?> GetTeamMembersAsync(
        string teamName,
        CancellationToken cancellationToken)
    {
        // Step 1: Get the MembersNames from the Teams collection
        var team = await _collection
            .Find(t => t.TeamName == teamName)
            .FirstOrDefaultAsync(cancellationToken);

        if (team is null) return null;

        // Step 2: Query the Users collection using the $in operator
        var filter = Builders<AppUser>.Filter.In(u => u.Id, team.MembersIds);
        var teamMembers = await _collectionAppUser
            .Find(filter)
            .ToListAsync(cancellationToken);

        return teamMembers;
    }

    public async Task<TeamStatus> AddMemberAsync(ObjectId userId, string targetMemberUserName, string targetTeamName,
        CancellationToken cancellationToken)
    {
        TeamStatus tS = new();

        Team team = await _collection.Find(t => t.TeamName == targetTeamName).FirstOrDefaultAsync(cancellationToken);

        if (team is null)
        {
            tS.IsTargetTeamNotFound = true;
            return tS;
        }

        if (team.CreatorId != userId)
        {
            tS.IsNotTheCreator = true;
            return tS;
        }

        AppUser user = await _collectionAppUser.Find(doc => doc.NormalizedUserName == targetMemberUserName.ToUpper())
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            tS.IsTargetMemberNotFound = true;
            return tS;
        }

        ObjectId memberId = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == targetMemberUserName.ToUpper())
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId == memberId)
        {
            tS.IsJoiningThemself = true;
            return tS;
        }

        Team teamContainingMember = await _collection.Find(t => t.MembersUserNames.Contains(targetMemberUserName))
            .FirstOrDefaultAsync(cancellationToken);

        if (teamContainingMember is not null)
        {
            tS.IsAlreadyJoined = true;
            return tS;
        }

        UpdateDefinition<Team> updateResult = Builders<Team>.Update
            .AddToSet(doc => doc.MembersIds, memberId)
            .AddToSet(doc => doc.MembersUserNames, targetMemberUserName);

        await _collection.UpdateOneAsync(doc => doc.Id == team.Id, updateResult, null, cancellationToken);

        UpdateDefinition<AppUser> userUpdateRes = Builders<AppUser>.Update
            .Set(doc => doc.EnrolledTeam, team.Id);

        await _collectionAppUser.UpdateOneAsync(doc => doc.Id == memberId, userUpdateRes, null, cancellationToken);

        tS.IsSuccess = true;

        return tS;
    }

    // public async Task<TeamStatus> RemoveMemberAsync(ObjectId userId, string targetMemberUserName, string targetTeamName,
    //     CancellationToken cancellationToken)
    // {
    //     TeamStatus tS = new();
    //
    //     Team team = await _collection.Find(t => t.TeamName == targetTeamName).FirstOrDefaultAsync(cancellationToken);
    //
    //     if (team is null)
    //     {
    //         tS.IsTargetTeamNotFound = true;
    //         return tS;
    //     }
    //
    //     if (team.CreatorId != userId)
    //     {
    //         tS.IsNotTheCreator = true;
    //         return tS;
    //     }
    //
    //     AppUser user = await _collectionAppUser.Find(doc => doc.NormalizedUserName == targetMemberUserName)
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     if (user is null)
    //     {
    //         tS.IsTargetTeamNotFound = true;
    //         return tS;
    //     }
    //
    //     ObjectId memberId = await _collectionAppUser.AsQueryable()
    //         .Where(doc => doc.NormalizedUserName == targetMemberUserName.ToUpper())
    //         .Select(doc => doc.Id)
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     if (userId == memberId)
    //     {
    //         tS.IsRemovingThemself = true;
    //         return tS;
    //     }
    //
    //     Team teamContainingMember = await _collection.Find(t => t.MembersUserNames.Contains(targetMemberUserName))
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     if (teamContainingMember is null)
    //     {
    //         tS.IsNotTeamMember = true;
    //         return tS;
    //     }
    //     
    //     
    // }
    public async Task<string?> GetTeamNameByIdAsync(ObjectId userId, CancellationToken cancellationToken)
    {
        string teamName = await _collection.AsQueryable()
            .Where(doc => doc.CreatorId == userId)
            .Select(doc => doc.TeamName)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamName is null)
            return null;

        return teamName;
    }

    public async Task<OperationResult> AssignCaptainAsync(ObjectId coachId, string targetUserName,
        CancellationToken cancellationToken)
    {
        string? coachUserName = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.Id == coachId)
            .Select(doc => doc.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        if (coachUserName is null)
        {
            return new OperationResult(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.CoachNotFound,
                    Message: "Coach not found."
                )
            );
        }

        // Find the coach doc
        var coachUser = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == coachUserName.ToUpper())
            .FirstOrDefaultAsync(cancellationToken);

        // Get the team created by the coach
        Team? coachTeam = await _collection.AsQueryable()
            .Where(t => t.CreatorId == coachUser.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (coachTeam is null)
        {
            return new OperationResult(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.CoachHasNoTeam,
                    Message: "Coach has no team."
                )
            );
        }

        if (!coachTeam.TeamCaptainId.Equals(ObjectId.Empty))
        {
            return new OperationResult(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.OnlyOneCaptain,
                    Message: "Only one captain is allowed."
                )
            );
        }

        // Find the target user (the player to be assigned as captain)
        var targetUser = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == targetUserName.ToUpper())
            .FirstOrDefaultAsync(cancellationToken);

        if (targetUser is null)
        {
            return new OperationResult(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.UserNotFound,
                    Message: "User not found."
                )
            );
        }

        // Get the target user's enrolled team
        ObjectId? userEnrolledTeamId = targetUser.EnrolledTeam;

        if (userEnrolledTeamId is null)
        {
            return new OperationResult(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.NotInTeam,
                    Message: "User is not in any team."
                )
            );
        }

        // Ensure the target user belongs to the coach's team
        if (userEnrolledTeamId != coachTeam.Id)
        {
            return new OperationResult(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.NotTeamMember,
                    Message: "User is not team member."
                )
            );
        }

        // Check if the user is already a captain
        var hasRole = await _userManager.IsInRoleAsync(targetUser, "captain");
        if (hasRole)
        {
            return new OperationResult(
                IsSuccess: false,
                Error: new CustomError(
                    Code: ErrorCode.AlreadyCaptain,
                    Message: "User is already a captain."
                )
            );
        }

        // Assign the captain role
        await _userManager.AddToRoleAsync(targetUser, "captain");

        UpdateDefinition<AppUser> updateResult = Builders<AppUser>.Update
            .Set(doc => doc.IsCaptain, true);

        UpdateDefinition<Team> updateTeamCap = Builders<Team>.Update
            .Set(team => team.TeamCaptainId, targetUser.Id);

        await _collection.UpdateOneAsync(
            team => team.Id == coachTeam.Id, // Find the correct team
            updateTeamCap,
            null,
            cancellationToken
        );

        await _collectionAppUser.UpdateOneAsync(doc => doc.Id == targetUser.Id, updateResult, null, cancellationToken);

        return new OperationResult(
            IsSuccess: true,
            Message: $"{targetUserName} assigned to captain."
        );
    }

    public async Task<CaptainStatus> RemoveCaptainAsync(ObjectId coachId, string targetUserName,
        CancellationToken cancellationToken)
    {
        CaptainStatus cS = new();

        string? coachUserName = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.Id == coachId)
            .Select(doc => doc.UserName)
            .FirstOrDefaultAsync(cancellationToken);

        if (coachUserName is null)
        {
            cS.CoachNotFound = true;
            return cS;
        }

        // Find the coach doc
        var coachUser = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == coachUserName.ToUpper())
            .FirstOrDefaultAsync(cancellationToken);

        // Get the team created by the coach
        Team? coachTeam = await _collection.AsQueryable()
            .Where(t => t.CreatorId == coachUser.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (coachTeam is null)
        {
            cS.CoachHasNoTeam = true;
            return cS;
        }

        // Find the target user (the player to be assigned as captain)
        var targetUser = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == targetUserName.ToUpper())
            .FirstOrDefaultAsync(cancellationToken);

        if (targetUser is null)
        {
            cS.UserNotFound = true;
            return cS;
        }

        // Get the target user's enrolled team
        ObjectId? userEnrolledTeamId = targetUser.EnrolledTeam;

        if (userEnrolledTeamId is null)
        {
            cS.NotInTeam = true;
            return cS;
        }

        // Ensure the target user belongs to the coach's team
        if (userEnrolledTeamId != coachTeam.Id)
        {
            cS.NotTeamMember = true;
            return cS;
        }

        // Check if the user is not captain
        var hasRole = await _userManager.IsInRoleAsync(targetUser, "captain");
        if (!hasRole)
        {
            cS.IsNotCaptain = true;
            return cS;
        }

        // Assign the captain role
        await _userManager.RemoveFromRoleAsync(targetUser, "captain");

        UpdateDefinition<AppUser> updateResult = Builders<AppUser>.Update
            .Set(doc => doc.IsCaptain, false);

        await _collectionAppUser.UpdateOneAsync(doc => doc.Id == targetUser.Id, updateResult, null, cancellationToken);

        UpdateDefinition<Team> updateTeamCap = Builders<Team>.Update
            .Set(doc => doc.TeamCaptainId, ObjectId.Empty);

        await _collection.UpdateOneAsync(
            team => team.Id == coachTeam.Id, // Find the correct team
            updateTeamCap,
            null,
            cancellationToken
        );

        cS.IsSuccess = true;

        return cS;
    }

    public async Task<OperationResult<ShowTeamDto>> UpdateVerifiedStatus(ObjectId teamId,
        CancellationToken cancellationToken)
    {
        Team team = await _collection.Find(t => t.Id == teamId).FirstOrDefaultAsync(cancellationToken);

        if (team is null)
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
            .Set(s => s.Status, Status.Verified);

        await _collection.UpdateOneAsync(t => t.Id == teamId, statusResult, null, cancellationToken);

        Team verifiedTeam = await _collection.Find(t => t.Id == team.Id).FirstOrDefaultAsync(cancellationToken);

        return new OperationResult<ShowTeamDto>(
            true,
            Result: Mappers.ConvertTeamToShowTeamDto(verifiedTeam)
        );
    }

    public Task<OperationResult<ShowTeamDto>> UpdateRejectStatus(ObjectId teamId, UpdateRejectStatus reason,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}