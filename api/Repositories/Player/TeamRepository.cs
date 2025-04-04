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
        _collection = dbName.GetCollection<Team>(AppVariablesExtensions.collectionTeams);
        _collectionAppUser = dbName.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);

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

    public async Task<TeamStatus> UpdateTeamAsync(
        ObjectId userId,
        UpdateTeamDto userInput, string targetTeamName, CancellationToken cancellationToken)
    {
        TeamStatus tS = new();

        ObjectId teamId = await _collection.AsQueryable()
            .Where(doc => doc.TeamName == targetTeamName)
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        Team teamName = await _collection.Find(t => t.TeamName == targetTeamName)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamName is null)
        {
            tS.IsTargetTeamNotFound = true;
            return tS;
        }

        ObjectId memberId = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == userInput.UserName.ToUpper())
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId == memberId)
        {
            tS.IsJoiningThemself = true;
            return tS;
        }

        AppUser user = await _collectionAppUser.Find(u => u.Id == memberId).FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            tS.IsTargetMemberNotFound = true;
            return tS;
        }

        Team team = await _collection.Find(t => t.MembersUserNames.Contains(userInput.UserName))
            .FirstOrDefaultAsync(cancellationToken);

        if (team is not null)
        {
            tS.IsAlreadyJoined = true;
            return tS;
        }

        UpdateDefinition<Team> updatedTeam = Builders<Team>.Update
            .AddToSet(t => t.MembersIds, memberId)
            .AddToSet(t => t.MembersUserNames, userInput.UserName?.ToLower().Trim())
            .Set(t => t.TeamName, userInput.TeamName?.ToLower().Trim())
            .Set(t => t.TeamLevel, userInput.TeamLevel?.ToLower().Trim())
            .Set(t => t.Achievements, userInput.Achievements?.ToLower().Trim())
            .Set(t => t.GamesPlayed, userInput.GamesPlayed)
            .Set(t => t.GamesWon, userInput.GamesWon)
            .Set(t => t.GamesLost, userInput.GamesLost);

        await _collection.UpdateOneAsync(
            doc => doc.Id == teamId, updatedTeam, null, cancellationToken
        );

        Team team1 = await _collection.Find(t => t.MembersUserNames.Contains(userInput.UserName))
            .FirstOrDefaultAsync(cancellationToken);

        if (team1 == null)
        {
            tS.IsTargetTeamNotFound = true;
            return tS;
        }

        EnrolledTeam? teamMap = Mappers.ConvertTeamToEnrolledTeamDto(team1);

        UpdateDefinition<AppUser> updatedUser = Builders<AppUser>.Update
            .AddToSet(appUser => appUser.EnrolledTeams, teamMap);

        await _collectionAppUser.UpdateOneAsync<AppUser>(appUser =>
            appUser.Id == memberId, updatedUser, null, cancellationToken);

        tS.IsSuccess = true;

        return tS;
    }

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

    public async Task<List<AppUser>> GetTeamMembersAsync(
        string teamName,
        CancellationToken cancellationToken)
    {
        // Step 1: Get the MembersNames from the Teams collection
        var team = await _collection
            .Find(t => t.TeamName == teamName)
            .FirstOrDefaultAsync(cancellationToken);

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
        
        Team team1 = await _collection.Find(t => t.MembersUserNames.Contains(targetMemberUserName))
            .FirstOrDefaultAsync(cancellationToken);

        if (team1 is not null)
        {
            tS.IsAlreadyJoined = true;
            return tS;
        }

        UpdateDefinition<Team> updateResult = Builders<Team>.Update
            .AddToSet(doc => doc.MembersIds, memberId)
            .AddToSet(doc => doc.MembersUserNames, targetMemberUserName);
        
        await _collection.UpdateOneAsync(doc => doc.Id == team.Id, updateResult, null, cancellationToken);
        
        tS.IsSuccess = true;

        return tS;
    }

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
    
    public async Task<bool?> AssignCaptainAsync(string targetUserName, CancellationToken cancellationToken)
    {
        // Find the target user by username
        var targetUser = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == targetUserName.ToUpper())
            .FirstOrDefaultAsync(cancellationToken);

        if (targetUser is null)
            return null; // User not found

        // Check if the user already has the 'captain' role
        var hasRole = await _userManager.IsInRoleAsync(targetUser, "captain");
        if (hasRole)
            return false; // Already a captain

        // Assign the captain role
        var result = await _userManager.AddToRoleAsync(targetUser, "captain");
        return result.Succeeded;
    }

    // public async Task<bool?> AssignCaptainAsync(ObjectId userId, string targetUserName, CancellationToken cancellationToken)
    // {
    //     ObjectId? targetUserId = await _collectionAppUser.AsQueryable()
    //         .Where(doc => doc.NormalizedUserName == targetUserName.ToUpper())
    //         .Select(doc => doc.Id)
    //         .FirstOrDefaultAsync(cancellationToken);
    //
    //     if (targetUserId is null)
    //         return null;
    //
    //     var hasRole = await _userManager.IsInRoleAsync(targetUserId, "captain");
    //     if (hasRole)
    //         return false;
    //     
    //     var result = await _userManager.AddToRoleAsync(targetUserId, "captain");
    //     return result.Succeeded;
    // }
}