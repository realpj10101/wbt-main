using System.Collections.Concurrent;
using api.Interfaces;
using api.Interfaces.Teams;
using api.Models;
using Microsoft.AspNetCore.SignalR;

namespace api.Hub;

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private ITeamMessagingRepository _teamMessagingRepository;

    private static readonly ConcurrentDictionary<string, string> _onlineUsers = new();
    private static readonly ConcurrentDictionary<string, DateTime> _lastSeenTimes = new();
    public ChatHub(ITeamMessagingRepository teamMessagingRepository)
    {
        _teamMessagingRepository = teamMessagingRepository;
    }

    public override async Task OnConnectedAsync()
    {
        string userName = (Context.User?.Identity?.Name ?? Context.ConnectionId).ToLowerInvariant(); ;

        _onlineUsers[userName] = Context.ConnectionId;

        // If the user was previously marked as offline, remove their last seen time
        // because they are now online.
        _lastSeenTimes.TryRemove(userName, out _);

        // Notify clients that a user came online
        await Clients.All.SendAsync("UserOnline", userName);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userEntry = _onlineUsers.FirstOrDefault(user => user.Value == Context.ConnectionId);

        // Check if a user was found for connection
        if (!string.IsNullOrEmpty(userEntry.Key))
        {
            string userName = userEntry.Key;

            // Remove the user from online users list
            _onlineUsers.TryRemove(userName, out _);

            var lastSeen = DateTime.UtcNow;
            _lastSeenTimes[userName] = lastSeen;

            await Clients.All.SendAsync("UserOffline", userName.ToLowerInvariant(), lastSeen);
        }

        await base.OnDisconnectedAsync(exception);
    }
    public async Task SendMessage(string userName, string message, string teaName)
    {
        MessageSenderDto sender = new(
            SenderUserName: userName,
            Message: message
        );

        await _teamMessagingRepository.SavedMessageAsync(sender, teaName);

        var timeStamp = DateTime.UtcNow;

        await Clients.All.SendAsync("ReceiveMessage", userName.ToLowerInvariant(), message, timeStamp);
    }

    public Task<List<UserStatusDto>> GetOnlineUsers()
    {
        var allKnownUsers = new HashSet<string>();

        // Add all currentl online users to set
        foreach (var userName in _onlineUsers.Keys)
        {
            allKnownUsers.Add(userName.ToLowerInvariant());
        }

        // Add users for whom we have a last seen time (they are currently offline)
        foreach (var userName in _lastSeenTimes.Keys)
        {
            allKnownUsers.Add(userName.ToLowerInvariant());
        }

        var userStatuses = new List<UserStatusDto>();

        // Iterate through all known users to build their status Dtos
        foreach (var userName in allKnownUsers)
        {
            // Check if the user is currently in the online users dictionary
            var isOnline = _onlineUsers.ContainsKey(userName);
            DateTime? lastSeen = null;

            // Check if user is not online, try get their last seen time from the _lastSeenTimes dictionary
            if (!isOnline && _lastSeenTimes.TryGetValue(userName, out var storedLastSeen))
            {
                lastSeen = storedLastSeen;
            }

            userStatuses.Add(new UserStatusDto
            {
                UserName = userName,
                IsOnline = isOnline,
                LastSeen = lastSeen
            });
        }

        return Task.FromResult(userStatuses);
    }
}