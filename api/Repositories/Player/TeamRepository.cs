using api.DTOs.Team_DTOs;
using api.Extensions;
using api.Interfaces.Team;
using api.Models.Helpers;

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

    public TeamRepository(
        IMongoClient client, IMyMongoDbSettings dbSettings,
        ITokenService tokenService, ILogger<TeamRepository> logger, IPlayerUserRepository playerUserRepository
    )
    {
        _client = client;
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<Team>(AppVariablesExtensions.collectionTeams);
        _collectionAppUser = dbName.GetCollection<AppUser>(AppVariablesExtensions.collectionUsers);
        
        _tokenService = tokenService;
        
        _playerUserRepository = playerUserRepository;
        
        _logger = logger;
    }
    
    #endregion
    
    // Create team and add member in
    public async Task<ShowTeamDto?> CreateAsync(
        ObjectId userId,
        CreateTeamDto userInput,
        CancellationToken cancellationToken)
    {
        Team? team = Mappers.ConvertCreateTeamDtoToTeam(userId, userInput);
        
        await _collection.InsertOneAsync(team, cancellationToken);

        if (team is not null)
        {
            ShowTeamDto showTeamDto = Mappers.ConvertTeamToShowTeamDto(team);

            return showTeamDto;
        }

        return null;
    }

    public async Task<bool> UpdateTeamAsync(
        UpdateTeamDto userInput, string targetTeamName, CancellationToken cancellationToken)
    {
        ObjectId teamId = await _collection.AsQueryable()
            .Where(doc => doc.TeamName == targetTeamName)
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        ObjectId memberId = await _collectionAppUser.AsQueryable()
            .Where(doc => doc.NormalizedUserName == userInput.UserName.ToUpper())
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        UpdateDefinition<Team> updatedTeam = Builders<Team>.Update
            .AddToSet(t => t.MembersIds, memberId)
            .Set(t => t.TeamName, userInput.TeamName)
            .Set(t => t.TeamLevel, userInput.TeamLevel)
            .Set(t => t.Achievements, userInput.Achievements)
            .Set(t => t.GamesPlayed, userInput.GamesPlayed)
            .Set(t => t.GamesWon, userInput.GamesWon)
            .Set(t => t.GamesLost, userInput.GamesLost);

        UpdateResult updatedResult = await _collection.UpdateOneAsync(
            doc => doc.Id == teamId, updatedTeam, null, cancellationToken
        );

        return updatedResult.ModifiedCount == 1;
    }
}