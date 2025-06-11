namespace api.Hub.Interfaces;

public interface IPresenceTrackerService
{
    public Task SaveConnectedUserAsync(ObjectId userId, string connectionId, CancellationToken cancellationToken);
    public Task<IEnumerable<OnlineUserDto>> GetOnlineUsersDtosAsync(CancellationToken cancellationToken);
    public Task RemoveDisconnectedUserAsync(string userName, string connectionId);
}