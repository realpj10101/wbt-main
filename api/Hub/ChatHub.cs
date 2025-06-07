using api.Interfaces;
using api.Interfaces.Teams;
using api.Models;
using Microsoft.AspNetCore.SignalR;

namespace api.Hub;

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private ITeamMessagingRepository _teamMessagingRepository;
    public ChatHub(ITeamMessagingRepository teamMessagingRepository)
    {
        _teamMessagingRepository = teamMessagingRepository;
    }

    public async Task SendMessage(string userName, string message, string teaName)
    {
        MessageSenderDto sender = new(
            SenderUserName: userName,
            Message: message
        );

        await _teamMessagingRepository.SavedMessageAsync(sender, teaName);

        await Clients.All.SendAsync("ReceiveMessage", userName, message);
    }
}