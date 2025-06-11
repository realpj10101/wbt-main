using api.Extensions;
using api.Hub.Interfaces;

namespace api.Hub.Services;

public class PresenceTrackerService : IPresenceTrackerService
{
    private readonly IMongoCollection<AppUser> _collection;
    
    public PresenceTrackerService(IMongoClient client, IMyMongoDbSettings dbSettings)
    {
        IMongoDatabase? dbName = client.GetDatabase(dbSettings.DatabaseName) ?? throw new ArgumentNullException(nameof(dbSettings));
        _collection = dbName.GetCollection<AppUser>(AppVariablesExtensions.CollectionUsers);
    }
    
    public async Task SaveConnectedUserAsync(ObjectId userId, string connectionId, CancellationToken cancellationToken)
    {
        UpdateDefinition<AppUser> updateDefinition = Builders<AppUser>.Update
            .AddToSet(appUser => appUser.ConnectionsPresence, connectionId);

        await _collection.UpdateOneAsync(appUser => appUser.Id == userId, updateDefinition, null, cancellationToken);
    }

    public async Task<IEnumerable<OnlineUserDto>> GetOnlineUsersDtosAsync(CancellationToken cancellationToken)
    {
        IEnumerable<AppUser> appUsers = await _collection.Find(appUser => appUser.ConnectionsPresence.Any()).ToListAsync(cancellationToken);

        List<OnlineUserDto> onlineUserDtos = [];

        foreach (AppUser appUser in appUsers)
        {
            onlineUserDtos.Add(Mappers.ConvertAppUserToOnlineStatusDto(appUser));
        }

        return onlineUserDtos;
    }

    public async Task RemoveDisconnectedUserAsync(string userName, string connectionId)
    {
        UpdateDefinition<AppUser> updateDefinition = Builders<AppUser>.Update
            .Pull(appUser => appUser.ConnectionsPresence, connectionId);

        await _collection.UpdateOneAsync(appUser => appUser.NormalizedUserName == userName, updateDefinition);
    }
}