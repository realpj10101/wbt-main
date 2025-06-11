using api.DTOs.Helpers;
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

    public async Task<MessageSenderDto?> SavedMessageAsync(MessageSenderDto message, string temaName)
    {
        ObjectId? teamId = _teamCollection.AsQueryable()
            .Where(doc => doc.TeamName == temaName)
            .Select(doc => doc.Id)
            .FirstOrDefault();

        if (teamId is null)
            return null;

        ChatMessage userMessage = Mappers.ConvertMessageSenderDtoToChatMessageDto(message, teamId);

        await _collection.InsertOneAsync(userMessage);

        return message;
    }

    public async Task<List<ChatMessage>> GetAllMessagesAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<OperationResult> DeleteAllMessagesAsync(string teamName, CancellationToken cancellationToken)
    {
        ObjectId? teamId = await _teamCollection.AsQueryable()
            .Where(doc => doc.TeamName.ToUpper() == teamName.ToUpper())
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamId is null || teamId.Equals(ObjectId.Empty))
        {
            return new OperationResult(
                false,
                Error: new CustomError(
                    ErrorCode.TeamNotFound,
                    "Target team not found"
                )
            );
        }

        ObjectId? chatId = await _collection.AsQueryable()
            .Where(doc => doc.TeamId == teamId)
            .Select(doc => doc.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (chatId is null || chatId.Equals(ObjectId.Empty))
        {
            return new OperationResult(
                false,
                Error: new CustomError(
                    ErrorCode.ChatNotFound,
                    "Target chat not found"
                )
            );
        }


        DeleteResult dS = _collection.DeleteMany(doc => doc.TeamId == teamId);

        return new OperationResult(
            true
        );
    }
}