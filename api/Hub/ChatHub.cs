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
    public ChatHub(ITeamMessagingRepository teamMessagingRepository)
    {
        _teamMessagingRepository = teamMessagingRepository;
    }

    public override async Task OnConnectedAsync()
    {
        string userName = (Context.User?.Identity?.Name ?? Context.ConnectionId).ToLowerInvariant(); ;

        _onlineUsers[userName] = Context.ConnectionId;

        // Notify clients that a user came online
        await Clients.All.SendAsync("UserOnline", userName);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = _onlineUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (!string.IsNullOrEmpty(user.Key))
        {
            _onlineUsers.TryRemove(user.Key, out _);

            // Store last seen time
            var lastSeen = DateTime.UtcNow;

            await Clients.All.SendAsync("UserOffline", user.Key.ToLowerInvariant(), lastSeen);
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

    public Task<List<string>> GetOnlineUsers()
    {
        var usersLowercase = _onlineUsers.Keys.Select(k => k.ToLowerInvariant()).ToList();
        return Task.FromResult(usersLowercase);
    }
}