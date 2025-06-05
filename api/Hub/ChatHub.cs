using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.SignalR;

namespace api.Hub;

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
{
    private readonly IChatRepository _chatRepository;

    public ChatHub(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task SendMessage(string userName, string message)
    {
        ChatMessage chatMessage = new(
            Id: null,
            UserName: userName,
            Message: message,
            TimeStamp: DateTime.UtcNow
        );

        await _chatRepository.SavedMessageAsync(chatMessage);

        await Clients.All.SendAsync("ReceiveMessage", userName, message);
    }
}