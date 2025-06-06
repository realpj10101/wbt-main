using api.Extensions;
using api.Interfaces.Teams;

namespace api.Repositories;

public class TeamMessagingRepository : ITeamMessagingRepository
{
    private readonly IMongoCollection<ChatMessage> _collection;
    private readonly IMongoCollection<Team> _teamCollection;

    public TeamMessagingRepository(IMongoClient client, IMyMongoDbSettings dbSettings)
    {
        var dbName = client.GetDatabase(dbSettings.DatabaseName);
        _collection = dbName.GetCollection<ChatMessage>(AppVariablesExtensions.CollectionChats);
        _teamCollection = dbName.GetCollection<Team>(AppVariablesExtensions.CollectionTeams);
    }
    
    public async Task<ChatMessage?> SavedMessageAsync(MessageSenderDto message, string temaName)
    {
        ObjectId? teamId = _teamCollection.AsQueryable()
            .Where(doc => doc.TeamName == temaName)
            .Select(doc => doc.Id)
            .FirstOrDefault();

        if (teamId is null)
            return null;
        
        ChatMessage userMessage = Mappers.ConvertMessageSenderDtoToChatMessageDto(message, teamId);
        
        await _collection.InsertOneAsync(userMessage);

        return userMessage;
    }

    public Task<List<ChatMessage>> GetAllMessagesAsync()
    {
        throw new NotImplementedException();
    }
}