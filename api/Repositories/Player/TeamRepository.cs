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
}